using System.Collections.Immutable;
using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private sealed partial class Resolution(GameSimulator rules, GameState state, Func<int, int>? randomChoice = null)
    {
        private readonly ImmutableArray<SimulationEvent>.Builder _events = ImmutableArray.CreateBuilder<SimulationEvent>();
        private readonly GameSimulator _rules = rules;
        private readonly Func<int, int>? _randomChoice = randomChoice;
        private GameState _state = state;
        private int _remainingSteps = rules._resolutionLimit;

        public SimulationResult Run(SimulationAction action, bool resolvesEffects)
        {
            switch (action.Kind)
            {
                case SimulationActionKind.EndTurn:
                    EndTurn(action.Actor);
                    break;
                case SimulationActionKind.PlayCard:
                    Play(action.Actor, action.Source!.Value, action.Target, action.Position, action.Choice, resolvesEffects);
                    break;
                case SimulationActionKind.Attack:
                    Attack(action.Actor, action.Source!.Value, action.Target!.Value);
                    break;
                case SimulationActionKind.UseHeroPower:
                    UseHeroPower(action.Actor, action.Source!.Value, action.Target, resolvesEffects);
                    break;
                case SimulationActionKind.ActivateLocation:
                    ActivateLocation(action.Actor, action.Source!.Value, action.Target, resolvesEffects);
                    break;
                case SimulationActionKind.ChooseOption:
                    ChoosePlayOption(action.Target!.Value);
                    break;
            }

            if (_state.PendingPlayChoice is null)
            {
                ResolveDeaths();
                FinishIfHeroesDied();
            }
            return new SimulationResult(_state, _events.ToImmutable());
        }

        private void EndTurn(PlayerId outgoing)
        {
            _events.Add(new(SimulationEventKind.TurnEnded, outgoing));
            RunTriggers(new(TriggerTiming.TurnEnded, outgoing), CaptureListeners());
            ResolveDeaths();
            FinishIfHeroesDied();
            if (_state.Outcome != MatchOutcome.InProgress)
                return;

            ThawAtTurnEnd(outgoing);
            // 移除效果和随后发生的死亡也可能添加本回合内到期的附魔。
            while (ExpireTurnEnchantments())
            {
                ResolveDeaths();
                FinishIfHeroesDied();
                if (_state.Outcome != MatchOutcome.InProgress)
                    return;
            }
            ExpireTurnHandCostModifiers();
            var previous = _state.Players[outgoing];
            _state = _state.UpdateResources(previous with { TemporaryMana = 0, CardsPlayedThisTurn = 0,
                PlayedElementalLastTurn = previous.PlayedElementalThisTurn, HeroHealthChangedThisTurn = false });
            _state = _state.PassTurn();
            var next = _state.Players[_state.ActivePlayer];
            _state = _state.UpdateResources(next with
            {
                ManaCrystals = next.ManaCrystals + Math.Min(1, next.MaximumManaCrystals - next.ManaCrystals),
                ManaSpent = 0, TemporaryMana = 0, OverloadLocked = next.OverloadOwed, OverloadOwed = 0,
                HeroPowerUsesThisTurn = 0, PlayedCardThisTurn = false, PlayedMinionThisTurn = false,
                CardsPlayedThisTurn = 0, PlayedElementalThisTurn = false,
                HeroHealthChangedThisTurn = false
            });
            foreach (var id in next.In(GameZone.Play))
            {
                var entity = _state.Entities[id];
                if (entity.Character is { } character)
                    _state = _state.UpdateCharacter(id, character with { AttacksThisTurn = 0, Exhausted = false });
                else if (entity.Location is { TurnsUntilReady: > 0 } location)
                    _state = _state.UpdateEntity(entity with { Location = location with { TurnsUntilReady = location.TurnsUntilReady - 1 } });
            }

            _events.Add(new(SimulationEventKind.TurnStarted, next.Id));
            RunTriggers(new(TriggerTiming.TurnStarted, next.Id), CaptureListeners());
            ResolveDeaths();
            FinishIfHeroesDied();
            if (_state.Outcome == MatchOutcome.InProgress)
                RunWork(new Stack<Work>([new DrawCards(next.Id, 1)]));
        }

        private void Play(PlayerId playerId, EntityId source, EntityId? target, int? position, string? choice, bool resolvesEffect)
        {
            var card = _rules.Card(_state.Entities[source]);
            var kind = card.Definition.Kind;
            var isMinion = kind == EntityKind.Minion;
            var option = choice is null ? null : card.Choices.Single(candidate => candidate.Id == choice);
            var effects = option?.Effects ?? card.OnPlay;
            var listeners = CaptureListeners();
            PayCardCost(playerId, source, card);
            var player = _state.Players[playerId];
            var cardsPlayedBefore = Math.Max(player.CardsPlayedThisTurn, player.PlayedCardThisTurn == true ? 1 : 0);
            _state = _state.UpdateResources(player with
            {
                PlayedCardThisTurn = true,
                PlayedMinionThisTurn = isMinion ? true : player.PlayedMinionThisTurn,
                CardsPlayedThisTurn = checked(cardsPlayedBefore + 1),
                PlayedElementalThisTurn = MatchesMinionType(card.Definition, MinionType.Elemental) ? true : player.PlayedElementalThisTurn
            });
            var index = position is int slot ? _rules.BoardIndex(_state, playerId, slot) : (int?)null;
            _state = _state.Move(source, new(playerId, card.IsSecret ? GameZone.Secret : GameZone.Play), index);
            _events.Add(new(SimulationEventKind.CardPlayed, playerId, source, target));

            if (kind == EntityKind.Spell && !_state.Players[_state.OpponentOf(playerId)].In(GameZone.Secret).IsEmpty)
            {
                RunTriggers(new(TriggerTiming.SpellCounterWindow, playerId, source), listeners);
                if (!CanCounterSpell(source))
                    return;
            }
            if (card.IsSecret)
                _events.Add(new(SimulationEventKind.SecretArmed, playerId, source));

            if (kind == EntityKind.Weapon)
            {
                var weapon = _state.Entities[source];
                _state = _state.UpdateEntity(weapon with { Weapon = card.CreateWeapon() });
                AddOverload(playerId, source, card.Overload);
                if (resolvesEffect)
                    ResolveEffects(new(source, playerId, target, CardsPlayedBefore: cardsPlayedBefore), effects);
                MarkEquippedWeaponForDestruction(playerId);
                SetEquippedWeapon(playerId, source);
                return;
            }

            if (kind == EntityKind.Location)
            {
                var entity = _state.Entities[source];
                _state = _state.UpdateEntity(entity with { Location = new(card.Definition.PrintedDurability) });
                _state = _rules.RefreshAuras(_state);
                AddOverload(playerId, source, card.Overload);
                if (resolvesEffect)
                    ResolveEffects(new(source, playerId, target, CardsPlayedBefore: cardsPlayedBefore), effects);
                return;
            }

            if (isMinion)
            {
                _state = _state.UpdateCharacter(source, card.CreateCharacter());
                FinishSummon(playerId, source);
            }

            var before = isMinion ? TriggerTiming.WhenMinionSummoned : TriggerTiming.BeforeSpellResolved;
            RunTriggers(new(before, playerId, source), listeners);
            AddOverload(playerId, source, card.Overload);
            ResolveDeaths();

            if (resolvesEffect)
                ResolveEffects(new(source, playerId, target, CardsPlayedBefore: cardsPlayedBefore), effects);
            if (resolvesEffect && card.ChooseAfterPlay is not null)
            {
                ResolveDeaths();
                if (OfferPlayChoice(playerId, source, target, card, listeners))
                    return;
            }
            CompletePlayedCard(playerId, source, card, listeners);
        }

        private void CompletePlayedCard(PlayerId playerId, EntityId source, SimulationCard card, ImmutableArray<Listener> listeners)
        {
            var isMinion = card.Definition.Kind == EntityKind.Minion;
            if (!isMinion && !card.IsSecret)
                _state = _state.Move(source, new(playerId, GameZone.Graveyard));
            ResolveDeaths();

            if (!isMinion || _state.LocationOf(source).Zone == GameZone.Play)
            {
                var after = isMinion ? TriggerTiming.AfterMinionPlayed : TriggerTiming.AfterSpellResolved;
                RunTriggers(new(after, _state.Entities[source].Controller, source), listeners);
            }
        }

        private void Attack(PlayerId player, EntityId source, EntityId target)
        {
            _events.Add(new(SimulationEventKind.Attack, player, source, target));
            if (!_state.Players[_state.OpponentOf(player)].In(GameZone.Secret).IsEmpty)
                target = ResolveAttackProposal(player, source, target);
            var isHero = source == _state.Players[player].Hero;
            if (isHero)
                RunTriggers(new(TriggerTiming.HeroAttacking, player, source), CaptureListeners());

            var attacker = _state.Entities[source];
            if (attacker.Character is not { } character || !IsLivingCharacter(source) || !IsLivingCharacter(target) ||
                _state.Players.Values.Any(combatant => !IsLivingCharacter(combatant.Hero)))
            {
                if (attacker.Character is { } spent)
                    _state = _state.UpdateCharacter(source, spent with { AttacksThisTurn = spent.AttacksThisTurn + 1 });
                _events.Add(new(SimulationEventKind.AttackCancelled, player, source, target));
                return;
            }

            var weapon = isHero ? _state.Players[player].EquippedWeapon : null;
            if (_rules.IsStealthed(attacker))
                _state = _state.UpdateEntity(attacker with
                {
                    Stealthed = false, Enchantments = [.. attacker.Enchantments.Where(layer => layer.GrantedTag != GAME_TAG.STEALTH)]
                });
            var retaliation = _rules.EffectiveAttack(_state, _state.Entities[target]);
            // A weapon may grant Poisonous or Lifesteal to hero combat damage.
            // Combat damage is simultaneous. Lifesteal must see retaliation
            // before healing, otherwise a full-health attacking hero wastes
            // the heal and incorrectly stays damaged after combat.
            var lifesteal = new List<(EffectContext Context, int Amount)>();
            var hit = Damage(source, target, _rules.EffectiveAttack(_state, attacker), weapon, lifesteal);
            var counter = Damage(target, source, retaliation, deferredLifesteal: lifesteal);
            var lifestealOutcomes = new List<Occurrence?>(lifesteal.Count);
            foreach (var (healingContext, healingAmount) in lifesteal)
                lifestealOutcomes.Add(HealFriendlyHero(healingContext, healingAmount));

            // 先记录双方受到的伤害，再处理伤害事件的触发队列。
            var work = new Stack<Work>();
            for (var i = lifestealOutcomes.Count - 1; i >= 0; i--)
                PushOccurrence(work, lifestealOutcomes[i]);
            PushOccurrence(work, counter);
            PushOccurrence(work, hit);
            RunWork(work);
            var afterDamage = _state.Entities[source].Character!;
            _state = _state.UpdateCharacter(source, afterDamage with { AttacksThisTurn = character.AttacksThisTurn + 1 });
            SpendWeaponDurability(weapon);
        }

        private void GainArmor(EffectContext context, int amount)
        {
            var hero = _state.Players[context.Controller].Hero;
            if (_state.LocationOf(hero).Zone != GameZone.Play)
                return;
            var character = _state.Entities[hero].Character!;
            _state = _state.UpdateCharacter(hero, character with { Armor = checked(character.Armor + amount) });
            _events.Add(new(SimulationEventKind.ArmorGained, context.Controller, context.Source, hero, amount));
        }

        private int CharacterMetricValue(GameEntity entity, CharacterMetric metric)
        {
            var character = entity.Character!;
            return metric switch
            {
                CharacterMetric.Health => character.Health,
                CharacterMetric.Damage => character.Damage,
                CharacterMetric.Attack => _rules.EffectiveAttack(_state, entity),
                CharacterMetric.Armor => character.Armor,
                _ => throw new InvalidOperationException("无法识别角色属性。")
            };
        }

        private void ApplyDamageTarget(Stack<Work> work, EffectContext context, DamageTarget target, int damageAmount,
            EnchantStatsEffect? enchantment = null)
        {
            if (target is DamageTarget.AllMinions or DamageTarget.EnemyMinions or DamageTarget.AllEnemies)
            {
                var enemy = _state.OpponentOf(context.Controller);
                var targets = target switch
                {
                    DamageTarget.AllEnemies => _state.Players[enemy].In(GameZone.Play).Where(id => _state.Entities[id].Character is not null),
                    DamageTarget.EnemyMinions => _rules.BoardMinions(_state, enemy),
                    _ => _state.Players.Keys.SelectMany(player => _rules.BoardMinions(_state, player))
                };
                var hits = targets
                    .OrderBy(id => _state.Entities[id].PlayOrder)
                    .Select(id => Damage(context.Source, id, damageAmount)).ToArray();
                for (var index = hits.Length - 1; index >= 0; index--)
                    PushOccurrence(work, hits[index]);
                return;
            }

            var targetId = target switch
            {
                DamageTarget.Selected => context.Selected!.Value,
                DamageTarget.Source => context.Source,
                DamageTarget.TriggerSubject => context.Subject!.Value,
                DamageTarget.FriendlyHero => _state.Players[context.Controller].Hero,
                DamageTarget.EnemyHero => _state.Players[_state.OpponentOf(context.Controller)].Hero,
                _ => throw new InvalidOperationException("无法识别伤害目标。")
            };
            var hit = Damage(context.Source, targetId, damageAmount);
            if (enchantment is not null)
                EnchantStats(context, enchantment);
            PushOccurrence(work, hit);
        }

        private void ApplyAdjacentAttackDamage(Stack<Work> work, EffectContext context, AdjacentAttackDamageEffect effect)
        {
            var centerId = (effect.Target == EffectTarget.Selected ? context.Selected : context.Subject)!.Value;
            if (!IsLivingCharacter(centerId))
                return;
            var center = _state.Entities[centerId];
            if (_rules.Card(center).Definition.Kind != EntityKind.Minion)
                return;
            var amount = _rules.EffectiveAttack(_state, center);
            if (amount <= 0)
                return;

            var slots = _rules.BoardSlots(_state, center.Controller).Select(id => _state.Entities[id]).ToArray();
            var position = Array.FindIndex(slots, entity => entity.Id == centerId);
            if (position < 0)
                return;

            var hits = new List<Occurrence?>(2);
            if (position > 0 && slots[position - 1].Character is not null && IsLivingCharacter(slots[position - 1].Id))
                hits.Add(Damage(centerId, slots[position - 1].Id, amount));
            if (position + 1 < slots.Length && slots[position + 1].Character is not null && IsLivingCharacter(slots[position + 1].Id))
                hits.Add(Damage(centerId, slots[position + 1].Id, amount));
            for (var index = hits.Count - 1; index >= 0; index--)
                PushOccurrence(work, hits[index]);
        }

        private void ExecuteEffect(Stack<Work> work, EffectContext context, CardEffect effect)
        {
            switch (effect)
            {
                case DamageEffect damage:
                    ApplyDamageTarget(work, context, damage.Target,
                        ModifiedEffectAmount(context, damage.Amount, damage: true), damage.Enchantment);
                    break;
                case ScaledDamageEffect scaled:
                    var valueTarget = ResolveConditionCharacter(context, scaled.ValueSource);
                    if (!IsLivingCharacter(valueTarget))
                        break;
                    var rawValue = CharacterMetricValue(_state.Entities[valueTarget!.Value], scaled.Metric);
                    var scaledAmount = ModifiedEffectAmount(context, rawValue, damage: true);
                    if (scaled.DestroyValueSource)
                        DestroyMinion(valueTarget.Value);
                    ApplyDamageTarget(work, context, scaled.Target, scaledAmount);
                    break;
                case AdjacentAttackDamageEffect adjacent:
                    ApplyAdjacentAttackDamage(work, context, adjacent);
                    break;
                case DrawEffect draw:
                    var drawer = draw.Recipient == CardRecipient.Controller ? context.Controller : _state.OpponentOf(context.Controller);
                    work.Push(new DrawCards(drawer, draw.Count));
                    break;
                case DrawMatchingEffect draw:
                    work.Push(new DrawCards(context.Controller, draw.Count, draw.CardFilter));
                    break;
                case ChangeCostTypeEffect change:
                    ChangeCostType(context, change);
                    break;
                case ChangeHandCostEffect change:
                    ChangeHandCost(context, change);
                    break;
                case RecruitEffect recruit:
                    work.Push(new RecruitCards(context.Controller, recruit.Count, recruit.CardFilter));
                    break;
                case CreateCardEffect create:
                    CreateCards(context, create);
                    break;
                case CopyDeckCardsEffect copy:
                    CopyDeckCards(context, copy);
                    break;
                case SummonDeckCopyEffect copy:
                    SummonDeckCopy(work, context, copy);
                    break;
                case DiscardEffect discard:
                    Discard(work, context, discard.Recipient, discard.Count, discard.Selection);
                    break;
                case DiscardHandEffect discard:
                    Discard(work, context, discard.Recipient, HandLimit, DiscardSelection.Random);
                    break;
                case SummonDiscardedMinionEffect:
                    SummonFromDiscard(work, context);
                    break;
                case GainArmorEffect armor:
                    GainArmor(context, armor.Amount);
                    break;
                case HealFriendlyHeroEffect heal:
                    PushOccurrence(work, HealFriendlyHero(context, ModifiedEffectAmount(context, heal.Amount, damage: false)));
                    break;
                case HealEffect heal:
                    HealCharacters(work, context, heal with { Amount = ModifiedEffectAmount(context, heal.Amount, damage: false) });
                    break;
                case EquipWeaponEffect equip:
                    CreateAndEquipWeapon(context.Controller, equip.CardId);
                    break;
                case DestroyEnemyWeaponEffect:
                    MarkEquippedWeaponForDestruction(_state.OpponentOf(context.Controller));
                    break;
                case EnchantStatsEffect enchant:
                    EnchantStats(context, enchant);
                    break;
                case SilenceEffect:
                    Silence(work, context);
                    break;
                case SummonEffect summon:
                    var position = ResolveSummonPosition(context, summon.Placement);
                    if (Summon(context.Controller, summon.CardId, position) is EntityId summoned)
                        PushSummonTriggers(work, context.Controller, summoned);
                    break;
                case SummonCopyEffect copy:
                    SummonCopy(work, context, copy);
                    break;
                case TakeControlEffect takeControl:
                    var targetMinion = (takeControl.Target == EffectTarget.Selected ? context.Selected : context.Subject)!.Value;
                    var destination = takeControl.ReturnToSourceOpponent
                        ? _state.OpponentOf(_state.Entities[context.Source].Controller)
                        : context.Controller;
                    TakeControl(context.Source, targetMinion, destination, takeControl.ReadyToAttack);
                    break;
                case ReturnMinionEffect returned:
                    ReturnMinion(work, context, returned);
                    break;
                case TransformMinionEffect transform:
                    TransformMinion(work, context, transform.CardId);
                    break;
                case TransformMinionByCostEffect transform:
                    TransformMinionByCost(work, context, transform.Increase);
                    break;
                case DestroyMinionEffect:
                    DestroyMinion(context.Selected!.Value);
                    break;
                case DestroyAllMinionsEffect destroyAll:
                    DestroyAllMinions(context, destroyAll);
                    break;
                case ResurrectFriendlyMinionEffect:
                    ResurrectFriendlyMinion(work, context.Controller);
                    break;
                case ResummonMinionEffect resummon:
                    ResummonMinion(work, context, resummon);
                    break;
                case FreezeEffect freeze:
                    Freeze(context, freeze);
                    break;
                case UnlockOverloadEffect:
                    UnlockOverload(context);
                    break;
                case CounterSpellEffect:
                    CounterSpell(context);
                    break;
                case PreventDamageEffect:
                    PreventDamage();
                    break;
                case SummonAttackDefenderEffect redirect:
                    SummonAttackDefender(work, context, redirect.CardId);
                    break;
                case RandomDamageEffect random:
                    ApplyRandomDamage(work, context, random);
                    break;
                case DestroyRandomEnemyMinionEffect:
                    DestroyRandomEnemyMinion(context);
                    break;
                case DestroyAllButOneMinionEffect:
                    DestroyAllButOneMinion();
                    break;
                case SummonRandomMinionEffect summon:
                    SummonRandomMinion(work, context.Controller, summon);
                    break;
                case SummonMatchingMinionEffect summon:
                    SummonMatchingMinion(work, context, summon);
                    break;
                case ChangeManaEffect mana:
                    ChangeMana(context, mana);
                    break;
                case ConditionalEffect conditional:
                    var branch = MeetsCondition(context, conditional.Condition) ? conditional.Then : conditional.Otherwise;
                    PushEffects(work, context, branch);
                    break;
            }
        }

        private int? ResolveSummonPosition(EffectContext context, SummonPlacement placement)
        {
            if (placement == SummonPlacement.Default)
                return context.DeathPosition;
            if (placement == SummonPlacement.Rightmost)
                return null;

            var slot = 0;
            foreach (var id in _rules.BoardSlots(_state, context.Controller))
            {
                if (id == context.Source)
                    return placement == SummonPlacement.LeftOfSource ? slot : slot + 1;
                slot++;
            }
            return null;
        }

        private EntityId? Summon(PlayerId player, string cardId, int? position, int? remainingHealth = null)
        {
            if (_rules.BoardSlotCount(_state, player) >= BoardLimit)
                return null;
            var id = new EntityId(_state.NextEntity);
            var character = _rules.GetCard(cardId).CreateCharacter();
            if (remainingHealth is int remaining)
                character = character with { Damage = Math.Max(0, character.MaximumHealth - remaining) };
            var entity = new GameEntity(id, cardId, player, player) { Character = character };
            var index = position is int slot ? _rules.BoardIndex(_state, player, slot) : _state.Players[player].In(GameZone.Play).Length;
            _state = _state.AddToPlay(entity, index);
            FinishSummon(player, id);
            return id;
        }

        private void FinishSummon(PlayerId player, EntityId id)
        {
            _state = _rules.RefreshAuras(_state);
            _events.Add(new(SimulationEventKind.Summoned, player, id));
        }

        private void SummonExistingMinion(PlayerId player, EntityId id)
        {
            var card = _rules.Card(_state.Entities[id]);
            _state = _state.Move(id, new(player, GameZone.Play));
            _state = _state.UpdateCharacter(id, card.CreateCharacter());
            FinishSummon(player, id);
        }

        private void TakeControl(EntityId source, EntityId target, PlayerId controller, bool readyToAttack = false)
        {
            var entity = _state.Entities[target];
            if (!IsLivingCharacter(target) || entity.Controller == controller)
                return;
            if (_rules.BoardSlotCount(_state, controller) >= BoardLimit)
            {
                RemoveImmediately(entity);
                return;
            }
            if (entity.Enchantments.Any(layer => layer.GrantedTag == GAME_TAG.CONTROLLER))
            {
                entity = RecalculateStats(entity with
                {
                    Enchantments = [.. entity.Enchantments.Where(layer => layer.GrantedTag != GAME_TAG.CONTROLLER)]
                }, entity.AuraGrants);
                _state = _state.UpdateEntity(entity);
            }
            _state = _state.Move(target, new(controller, GameZone.Play));
            _state = _state.UpdateCharacter(target, _state.Entities[target].Character! with { Exhausted = !readyToAttack });
            // Control changes the emitter's friendly side even though the
            // entity stays on the battlefield; refresh before later effects.
            _state = _rules.RefreshAuras(_state);
            _events.Add(new(SimulationEventKind.ControlChanged, controller, source, target));
        }

        private void ResolveDeaths()
        {
            while (true)
            {
                var dead = _state.Players.Values.SelectMany(player => player.In(GameZone.Play))
                    .Select(id => _state.Entities[id])
                    .Where(entity => entity.Character is { Health: <= 0 } || entity.Weapon is { Durability: 0 } ||
                        _pendingDestruction.Contains(entity.Id))
                    .OrderBy(entity => entity.PlayOrder).ToArray();
                if (dead.Length == 0 && _immediateDeaths.Count == 0)
                {
                    _state = _rules.RefreshAuras(_state);
                    return;
                }

                var listeners = CaptureListeners();
                var wave = new List<Death>(_immediateDeaths);
                _immediateDeaths.Clear();
                foreach (var entity in dead)
                    wave.Add(RemoveDeadEntity(entity));

                _state = _rules.RefreshAuras(_state);
                foreach (var death in wave.OrderBy(entry => entry.Entity.PlayOrder))
                {
                    var (entity, position) = death;
                    if (!entity.Enchantments.IsEmpty && _rules._enchantmentRemovalEffects.Count != 0)
                    {
                        var work = new Stack<Work>();
                        PushEnchantmentRemovalEffects(work, entity, entity.Enchantments, EnchantmentRemovalReason.LeftPlay);
                        RunWork(work);
                    }
                    if (position is null && entity.Weapon is null)
                        continue;
                    var timing = position is not null ? TriggerTiming.MinionDied : TriggerTiming.WeaponDestroyed;
                    var queue = Queue(new(timing, entity.Controller, entity.Id),
                        death.ListenersAtDeath ?? listeners);
                    var effects = entity.Silenced ? [] : _rules.Card(entity).Deathrattle;
                    if (!effects.IsEmpty)
                        queue = queue.Append(new(new(entity.Id, entity.Controller, DeathPosition: position),
                            entity.PlayOrder!.Value, effects));
                    foreach (var layer in entity.Enchantments)
                        if (layer.GrantedTag == GAME_TAG.DEATHRATTLE && layer.CardId is { } enchantment &&
                            _rules._enchantmentRemovalEffects.TryGetValue((enchantment, EnchantmentRemovalReason.Died), out var granted))
                            queue = queue.Append(new(new(entity.Id, entity.Controller, DeathPosition: position),
                                entity.PlayOrder!.Value, granted));
                    RunQueue(queue.OrderBy(trigger => trigger.PlayOrder));
                }
            }
        }

        private void FinishIfHeroesDied()
        {
            if (_state.Outcome != MatchOutcome.InProgress)
                return;
            var survivors = _state.Players.Values
                .Where(player => _state.LocationOf(player.Hero).Zone == GameZone.Play).ToArray();
            if (survivors.Length == _state.Players.Count)
                return;
            PlayerId? winner = survivors.Length == 1 ? survivors[0].Id : null;
            _state = _state.Finish(winner);
            _events.Add(new(SimulationEventKind.MatchEnded, winner));
        }

        private void SpendResolutionStep()
        {
            if (_remainingSteps-- <= 0)
                throw new InvalidOperationException("动作超出模拟器的结算预算。");
        }
    }
}
