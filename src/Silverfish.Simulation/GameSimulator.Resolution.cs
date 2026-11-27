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
        private EntityId? _activeAttackingHero;

        private bool IsImmuneAttackingHero(EntityId id) =>
            _activeAttackingHero == id &&
            _state.Players[_state.Entities[id].Controller].EquippedWeapon is EntityId eq &&
            _state.LocationOf(eq).Zone == GameZone.Play &&
            !_state.Entities[eq].Silenced &&
            _rules.Card(_state.Entities[eq]).HeroImmuneWhileAttacking;

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
            var pendingAdditions = _state.Players[outgoing].EndOfTurnHandAdditions;
            if (!pendingAdditions.IsEmpty)
            {
                _state = _state.UpdateResources(_state.Players[outgoing] with { EndOfTurnHandAdditions = [] });
                var hero = _state.Players[outgoing].Hero;
                foreach (var cardId in pendingAdditions)
                    TryCreateCard(cardId, outgoing, hero, CardPlacement.Hand);
            }
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
                PlayedElementalLastTurn = previous.PlayedElementalThisTurn, HeroHealthChangedThisTurn = false,
                MinionsMinimumOneHealthThisTurn = false });
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
            ConsumePlayedCardCostAuras(playerId, card);
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
                if (target is EntityId proposedTarget &&
                    _rules.Card(_state.Entities[proposedTarget]).Definition.Kind == EntityKind.Minion &&
                    !_state.Players[_state.OpponentOf(playerId)].In(GameZone.Secret).IsEmpty)
                {
                    target = ResolveSpellTargetProposal(playerId, proposedTarget);
                }
            }
            if (card.IsSecret)
                _events.Add(new(SimulationEventKind.SecretArmed, playerId, source));

            if (kind == EntityKind.Hero)
            {
                PlayHeroCard(playerId, source);
                RunTriggers(new(TriggerTiming.WhenCardPlayed, playerId, source), listeners);
                AddOverload(playerId, source, card.Overload);
                ResolveDeaths();
                if (resolvesEffect)
                    ResolveEffects(new(source, playerId, target, CardsPlayedBefore: cardsPlayedBefore), effects);
                ResolveDeaths();
                RunTriggers(new(TriggerTiming.AfterCardPlayed, playerId, source), listeners);
                return;
            }

            if (kind == EntityKind.Weapon)
            {
                var weapon = _state.Entities[source];
                _state = _state.UpdateEntity(weapon with { Weapon = card.CreateWeapon() });
                RunTriggers(new(TriggerTiming.WhenCardPlayed, playerId, source), listeners);
                AddOverload(playerId, source, card.Overload);
                ResolveDeaths();
                if (resolvesEffect)
                    ResolveEffects(new(source, playerId, target, CardsPlayedBefore: cardsPlayedBefore), effects);
                MarkEquippedWeaponForDestruction(playerId);
                SetEquippedWeapon(playerId, source);
                ResolveDeaths();
                RunTriggers(new(TriggerTiming.AfterCardPlayed, playerId, source), listeners);
                return;
            }

            if (kind == EntityKind.Location)
            {
                var entity = _state.Entities[source];
                _state = _state.UpdateEntity(entity with { Location = new(card.Definition.PrintedDurability) });
                _state = _rules.RefreshAuras(_state);
                RunTriggers(new(TriggerTiming.WhenCardPlayed, playerId, source), listeners);
                AddOverload(playerId, source, card.Overload);
                ResolveDeaths();
                if (resolvesEffect)
                    ResolveEffects(new(source, playerId, target, CardsPlayedBefore: cardsPlayedBefore), effects);
                ResolveDeaths();
                RunTriggers(new(TriggerTiming.AfterCardPlayed, playerId, source), listeners);
                return;
            }

            if (isMinion)
            {
                var updatedMinion = _state.Entities[source] with { Character = card.CreateCharacter() };
                if (!updatedMinion.Enchantments.IsEmpty)
                    updatedMinion = RecalculateStats(updatedMinion, []);
                _state = _state.UpdateCharacter(source, updatedMinion.Character!);
                FinishSummon(playerId, source);
            }

            RunTriggers(new(TriggerTiming.WhenCardPlayed, playerId, source), listeners);
            var before = isMinion ? TriggerTiming.WhenMinionSummoned : TriggerTiming.BeforeSpellResolved;
            RunTriggers(new(before, playerId, source), listeners);
            if (!isMinion && target is EntityId spellTarget)
                RunTriggers(new(TriggerTiming.TargetedBySpell, playerId, spellTarget), listeners);
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
            RunTriggers(new(TriggerTiming.AfterCardPlayed, playerId, source), listeners);
        }

        private void Attack(PlayerId player, EntityId source, EntityId target)
        {
            var isHero = source == _state.Players[player].Hero;
            _activeAttackingHero = isHero ? source : null;
            try
            {
                _events.Add(new(SimulationEventKind.Attack, player, source, target));
                if (!_state.Players[_state.OpponentOf(player)].In(GameZone.Secret).IsEmpty)
                    target = ResolveAttackProposal(player, source, target);
                target = MaybeRedirectOgreAttack(player, source, target);
                if (isHero)
                    RunTriggers(new(TriggerTiming.HeroAttacking, player, source), CaptureListeners());
                if (_rules._enchantmentAttackEffects.Count != 0 &&
                    _state.LocationOf(source).Zone == GameZone.Play)
                {
                    var layers = _state.Entities[source].Enchantments;
                    foreach (var layer in layers)
                        if (layer.CardId is { } cid && _rules._enchantmentAttackEffects.TryGetValue(cid, out var onAttack))
                            ResolveEffects(new(layer.Source, _state.Entities[layer.Source].Owner, Subject: source), onAttack);
                }

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
            var attackerPower = _rules.EffectiveAttack(_state, attacker);
            var hit = Damage(source, target, attackerPower, weapon, lifesteal);
            var cleaveHits = new List<Occurrence?>();
            if (!attacker.Silenced && _rules.Card(attacker).CleaveAdjacentOnAttack &&
                _rules.Card(_state.Entities[target]).Definition.Kind == EntityKind.Minion)
            {
                foreach (var adj in AdjacentMinions(target))
                    cleaveHits.Add(Damage(source, adj, attackerPower, weapon, lifesteal));
            }
            var counter = Damage(target, source, retaliation, deferredLifesteal: lifesteal);
            var lifestealOutcomes = new List<Occurrence?>(lifesteal.Count);
            foreach (var (healingContext, healingAmount) in lifesteal)
                lifestealOutcomes.Add(HealFriendlyHero(healingContext, healingAmount));

            // 先记录双方受到的伤害，再处理伤害事件的触发队列。
            var work = new Stack<Work>();
            for (var i = lifestealOutcomes.Count - 1; i >= 0; i--)
                PushOccurrence(work, lifestealOutcomes[i]);
            PushOccurrence(work, counter);
            for (var i = cleaveHits.Count - 1; i >= 0; i--)
                PushOccurrence(work, cleaveHits[i]);
            PushOccurrence(work, hit);
            RunWork(work);
            var afterDamage = _state.Entities[source].Character!;
            _state = _state.UpdateCharacter(source, afterDamage with { AttacksThisTurn = character.AttacksThisTurn + 1 });
            SpendWeaponDurability(weapon, target);
            }
            finally
            {
                _activeAttackingHero = null;
            }
        }

        private void GainArmor(EffectContext context, int amount)
        {
            var hero = _state.Players[context.Controller].Hero;
            if (_state.LocationOf(hero).Zone != GameZone.Play)
                return;
            var character = _state.Entities[hero].Character!;
            _state = _state.UpdateCharacter(hero, character with { Armor = checked(character.Armor + amount) });
            _events.Add(new(SimulationEventKind.ArmorGained, context.Controller, context.Source, hero, amount));
            RunTriggers(new(TriggerTiming.ArmorGained, context.Controller, hero, Amount: amount), CaptureListeners());
        }

        private EntityId MaybeRedirectOgreAttack(PlayerId player, EntityId source, EntityId target)
        {
            var attacker = _state.Entities[source];
            var isHero = source == _state.Players[player].Hero;
            var hasOgreChance = (!attacker.Silenced && _rules.Card(attacker).AttackWrongEnemyChance) ||
                (isHero && _state.Players[player].EquippedWeapon is EntityId weaponId && _rules.Card(_state.Entities[weaponId]).AttackWrongEnemyChance) ||
                (!isHero && _state.Players.Values.SelectMany(p => _rules.BoardMinions(_state, p.Id))
                    .Any(id => IsLivingCharacter(id) && !_state.Entities[id].Silenced && _rules.Card(_state.Entities[id]).AllMinionsAttackWrongEnemyChance));
            if (!hasOgreChance)
                return target;

            var enemy = _state.OpponentOf(player);
            var others = _state.Players[enemy].In(GameZone.Play)
                .Where(id => id != target && IsLivingCharacter(id) && !_rules.IsEntityImmune(_state, _state.Entities[id]))
                .OrderBy(id => _state.Entities[id].PlayOrder)
                .ToArray();
            if (others.Length == 0 || ChooseIndex(2) == 0)
                return target;

            return others[ChooseIndex(others.Length)];
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
            EnchantStatsEffect? enchantment = null, string? excludeCardId = null,
            bool undamagedOnly = false, MinionType? excludeMinionType = null, CardFilter? targetCardFilter = null)
        {
            if (target is DamageTarget.AllMinions or DamageTarget.EnemyMinions or DamageTarget.AllEnemies or
                DamageTarget.AllCharacters or DamageTarget.OtherCharacters)
            {
                var enemy = _state.OpponentOf(context.Controller);
                var targets = target switch
                {
                    DamageTarget.AllEnemies => _state.Players[enemy].In(GameZone.Play).Where(id => _state.Entities[id].Character is not null),
                    DamageTarget.EnemyMinions => _rules.BoardMinions(_state, enemy),
                    DamageTarget.AllCharacters => _state.Players.Keys.SelectMany(player => _state.Players[player].In(GameZone.Play))
                        .Where(id => _state.Entities[id].Character is not null),
                    DamageTarget.OtherCharacters => _state.Players.Keys.SelectMany(player => _state.Players[player].In(GameZone.Play))
                        .Where(id => id != context.Source && _state.Entities[id].Character is not null),
                    _ => _state.Players.Keys.SelectMany(player => _rules.BoardMinions(_state, player))
                };
                if (excludeCardId is not null)
                    targets = targets.Where(id => _state.Entities[id].CardId != excludeCardId);
                if (undamagedOnly)
                    targets = targets.Where(id => _state.Entities[id].Character?.Damage == 0);
                if (excludeMinionType is MinionType excludedRace)
                    targets = targets.Where(id => !MatchesMinionType(_rules.Card(_state.Entities[id]).Definition, excludedRace));
                if (targetCardFilter is not null)
                    targets = targets.Where(id => MatchesCardFilter(_rules.Card(_state.Entities[id]), targetCardFilter));
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

        private ImmutableArray<EntityId> AdjacentMinions(EntityId centerId)
        {
            if (!_state.Entities.TryGetValue(centerId, out var center) || _state.LocationOf(centerId).Zone != GameZone.Play)
                return [];
            var slots = _rules.BoardSlots(_state, center.Controller).Select(id => _state.Entities[id]).ToArray();
            var position = Array.FindIndex(slots, entity => entity.Id == centerId);
            if (position < 0)
                return [];

            var adjacent = ImmutableArray.CreateBuilder<EntityId>(2);
            if (position > 0 && slots[position - 1].Character is not null && IsLivingCharacter(slots[position - 1].Id))
                adjacent.Add(slots[position - 1].Id);
            if (position + 1 < slots.Length && slots[position + 1].Character is not null && IsLivingCharacter(slots[position + 1].Id))
                adjacent.Add(slots[position + 1].Id);
            return adjacent.ToImmutable();
        }

        private ImmutableArray<EntityId> TargetAndAdjacentMinions(EntityId centerId)
        {
            if (!IsLivingCharacter(centerId) || _rules.Card(_state.Entities[centerId]).Definition.Kind != EntityKind.Minion)
                return [];
            var adjacent = AdjacentMinions(centerId);
            return adjacent.Add(centerId).OrderBy(id => _state.Entities[id].PlayOrder).ToImmutableArray();
        }

        private void ApplySplashDamage(Stack<Work> work, EffectContext context, SplashDamageEffect effect)
        {
            var centerId = context.Selected!.Value;
            if (!IsLivingCharacter(centerId) || _rules.Card(_state.Entities[centerId]).Definition.Kind != EntityKind.Minion)
                return;

            var centerDamage = ModifiedEffectAmount(context, effect.CenterAmount, damage: true);
            var sideDamage = ModifiedEffectAmount(context, effect.AdjacentAmount, damage: true);
            var targets = AdjacentMinions(centerId)
                .Select(id => (Id: id, Amount: sideDamage))
                .Append((Id: centerId, Amount: centerDamage))
                .OrderBy(entry => _state.Entities[entry.Id].PlayOrder)
                .ToArray();
            var hits = targets.Select(entry => Damage(context.Source, entry.Id, entry.Amount)).ToArray();
            for (var index = hits.Length - 1; index >= 0; index--)
                PushOccurrence(work, hits[index]);
        }

        private void ApplyDynamicDraw(Stack<Work> work, EffectContext context, DynamicDrawEffect effect)
        {
            var count = effect.Mode switch
            {
                DynamicDrawMode.DamagedFriendlyCharacters => _state.Players[context.Controller].In(GameZone.Play)
                    .Count(id => IsLivingCharacter(id) && _state.Entities[id].Character is { Damage: > 0 }),
                DynamicDrawMode.MatchOpponentHandSize =>
                    _state.Players[_state.OpponentOf(context.Controller)].In(GameZone.Hand).Length -
                    _state.Players[context.Controller].In(GameZone.Hand).Length,
                _ => throw new InvalidOperationException("无法识别动态抽牌方式。")
            };
            if (count > 0)
                work.Push(new DrawCards(context.Controller, count));
        }

        private void ApplyDrawAndDamageByCost(Stack<Work> work, EffectContext context, DrawAndDamageByCostEffect effect)
        {
            var deck = _state.Players[context.Controller].In(GameZone.Deck);
            if (!deck.IsEmpty)
            {
                var printedCost = _rules.Card(_state.Entities[deck[0]]).Definition.PrintedCost;
                work.Push(new DealDrawnCostDamage(context, effect.Target, printedCost));
            }
            work.Push(new DrawCards(context.Controller, 1));
        }

        private void ApplyFillBoardSummon(Stack<Work> work, EffectContext context, FillBoardSummonEffect effect)
        {
            if (_rules.BoardSlotCount(_state, context.Controller) >= BoardLimit)
                return;

            var placement = effect.AlternateSides
                ? (effect.PlaceLeftNext ? SummonPlacement.LeftOfSource : SummonPlacement.RightOfSource)
                : SummonPlacement.Rightmost;
            var position = ResolveSummonPosition(context, placement);
            if (Summon(context.Controller, effect.CardId, position) is EntityId summoned)
            {
                if (_rules.BoardSlotCount(_state, context.Controller) < BoardLimit)
                    work.Push(new ApplyEffect(context, effect with { PlaceLeftNext = !effect.PlaceLeftNext }));
                PushSummonTriggers(work, context.Controller, summoned);
            }
        }

        private void ExecuteEffect(Stack<Work> work, EffectContext context, CardEffect effect)
        {
            switch (effect)
            {
                case DamageEffect damage:
                    ApplyDamageTarget(work, context, damage.Target,
                        ModifiedEffectAmount(context, damage.Amount, damage: true), damage.Enchantment, damage.ExcludeCardId,
                        damage.UndamagedOnly, damage.ExcludeMinionType, damage.TargetCardFilter);
                    break;
                case DealAttackAsDamageToAllMinionsEffect:
                    var allMinions = _state.Players.Keys
                        .SelectMany(player => _rules.BoardMinions(_state, player))
                        .OrderBy(id => _state.Entities[id].PlayOrder)
                        .ToArray();
                    var hits = new List<Occurrence?>();
                    foreach (var minionId in allMinions)
                    {
                        var atk = _rules.EffectiveAttack(_state, _state.Entities[minionId]);
                        if (atk > 0)
                            hits.Add(Damage(context.Source, minionId, ModifiedEffectAmount(context, atk, damage: true)));
                    }
                    for (var i = hits.Count - 1; i >= 0; i--)
                        PushOccurrence(work, hits[i]);
                    break;
                case EventAmountDamageEffect eventDamage:
                    if (context.EventAmount is int eventAmount && eventAmount > 0)
                        ApplyDamageTarget(work, context, eventDamage.Target,
                            ModifiedEffectAmount(context, eventAmount, damage: true));
                    break;
                case SplashDamageEffect splash:
                    ApplySplashDamage(work, context, splash);
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
                    work.Push(new DrawCards(drawer, draw.Count, CostDelta: draw.CostDelta,
                        CostEnchantmentId: draw.EnchantmentCardId,
                        CostSource: draw.CostDelta != 0 ? context.Source : null));
                    break;
                case DynamicDrawEffect dynamicDraw:
                    ApplyDynamicDraw(work, context, dynamicDraw);
                    break;
                case DrawAndDamageByCostEffect costDraw:
                    ApplyDrawAndDamageByCost(work, context, costDraw);
                    break;
                case DrawMatchingEffect draw:
                    work.Push(new DrawCards(context.Controller, draw.Count, draw.CardFilter));
                    break;
                case DrawAndTransformIfMinionEffect drawTransform:
                    DrawAndTransformIfMinion(work, context, drawTransform);
                    break;
                case DrawAndDiscountIfMatchesEffect drawDiscount:
                    DrawAndDiscountIfMatches(work, context, drawDiscount);
                    break;
                case DrawToHandSizeEffect drawToSize:
                    DrawToHandSize(work, context, drawToSize);
                    break;
                case MillDeckEffect mill:
                    MillDeck(context, mill);
                    break;
                case ChangeCostTypeEffect change:
                    ChangeCostType(context, change);
                    break;
                case ChangeHandCostEffect change:
                    ChangeHandCost(context, change);
                    break;
                case AddTemporaryCostAuraEffect tempCost:
                    ApplyTemporaryCostAura(context, tempCost);
                    break;
                case RecruitEffect recruit:
                    var recruitOwner = recruit.Recipient == CardRecipient.Controller
                        ? context.Controller : _state.OpponentOf(context.Controller);
                    work.Push(new RecruitCards(recruitOwner, recruit.Count, recruit.CardFilter));
                    break;
                case PutSecretFromDeckEffect secretDeck:
                    PutSecretFromDeck(context, secretDeck);
                    break;
                case SummonFromHandEffect summonHand:
                    SummonFromHand(work, context, summonHand);
                    break;
                case CreateCardEffect create:
                    CreateCards(context, create);
                    break;
                case CreateRandomCardEffect createRandom:
                    var randomRecipient = createRandom.Recipient == CardRecipient.Controller
                        ? context.Controller : _state.OpponentOf(context.Controller);
                    for (var i = 0; i < createRandom.Count; i++)
                        TryCreateCard(createRandom.CardIds[ChooseIndex(createRandom.CardIds.Length)], randomRecipient, context.Source, CardPlacement.Hand);
                    break;
                case CreateRandomMatchingCardEffect createMatching:
                    CreateRandomMatchingCard(context, createMatching);
                    break;
                case CreateRandomOpponentClassSpellEffect createOpponentSpell:
                    CreateRandomOpponentClassSpell(context, createOpponentSpell);
                    break;
                case CopyTriggerCardToHandEffect copyTrigger:
                    CopyTriggerCardToHand(context, copyTrigger);
                    break;
                case CopyTargetCardEffect copyTarget:
                    CopyTargetCard(context, copyTarget);
                    break;
                case CopyFriendlyBoardToHandEffect:
                    CopyFriendlyBoardToHand(context);
                    break;
                case CreateCardAtTurnEndEffect delayedCreate:
                    var delayedOwner = _state.Players[context.Controller];
                    _state = _state.UpdateResources(delayedOwner with
                    {
                        EndOfTurnHandAdditions = delayedOwner.EndOfTurnHandAdditions.Add(delayedCreate.CardId)
                    });
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
                case SetHeroRemainingHealthEffect setHeroHealth:
                    SetHeroRemainingHealth(context, setHeroHealth);
                    break;
                case ReplaceHeroPowerEffect replaceHeroPower:
                    ReplaceHeroPower(context.Controller, context.Source, replaceHeroPower.CardId);
                    break;
                case ReplaceHeroEffect replaceHero:
                    ReplaceHero(context.Controller, context.Source, replaceHero);
                    break;
                case EquipWeaponEffect equip:
                    CreateAndEquipWeapon(context.Controller, equip.CardId);
                    break;
                case EquipRandomWeaponEachPlayerEffect:
                    var weaponIds = _rules._cards.Values
                        .Where(c => c.Definition.Kind == EntityKind.Weapon)
                        .Select(c => c.Definition.CardId)
                        .OrderBy(id => id, StringComparer.Ordinal)
                        .ToArray();
                    if (weaponIds.Length > 0)
                    {
                        CreateAndEquipWeapon(context.Controller, weaponIds[ChooseIndex(weaponIds.Length)]);
                        CreateAndEquipWeapon(_state.OpponentOf(context.Controller), weaponIds[ChooseIndex(weaponIds.Length)]);
                    }
                    break;
                case ModifyWeaponEffect modifyWeapon:
                    ModifyWeapon(context, modifyWeapon);
                    break;
                case DestroyEnemyWeaponEffect destroyWeapon:
                    var enemyOwner = _state.OpponentOf(context.Controller);
                    if (_state.Players[enemyOwner].EquippedWeapon is EntityId enemyWeapon &&
                        _state.LocationOf(enemyWeapon).Zone == GameZone.Play &&
                        _state.Entities[enemyWeapon].Weapon is { Durability: > 0 } enemyWeaponStats)
                    {
                        MarkEquippedWeaponForDestruction(enemyOwner);
                        if (destroyWeapon.DrawDurability)
                            work.Push(new DrawCards(context.Controller, enemyWeaponStats.Durability));
                    }
                    break;
                case DestroyFriendlyWeaponDamageEffect flurry:
                    if (_state.Players[context.Controller].EquippedWeapon is EntityId friendlyWeapon &&
                        _state.LocationOf(friendlyWeapon).Zone == GameZone.Play &&
                        _state.Entities[friendlyWeapon].Weapon is { Durability: > 0 })
                    {
                        var weaponAttack = _rules.EffectiveWeaponAttack(_state, _state.Entities[friendlyWeapon]);
                        MarkEquippedWeaponForDestruction(context.Controller);
                        ApplyDamageTarget(work, context, flurry.Target,
                            ModifiedEffectAmount(context, weaponAttack, damage: true));
                    }
                    break;
                case ProtectFriendlyMinionsMinimumHealthEffect:
                    _state = _state.UpdateResources(_state.Players[context.Controller] with { MinionsMinimumOneHealthThisTurn = true });
                    break;
                case CopyTriggerSpellToOtherPlayerEffect:
                    if (context.Subject is EntityId castSpellId && _state.Entities.TryGetValue(castSpellId, out var castSpell))
                        TryCreateCard(castSpell.CardId, _state.OpponentOf(castSpell.Controller), context.Source, CardPlacement.Hand);
                    break;
                case EnchantStatsEffect enchant:
                    EnchantStats(context, enchant);
                    break;
                case RandomEnchantStatsEffect randomEnchant:
                    ApplyRandomEnchantStats(context, randomEnchant);
                    break;
                case ScaledEnchantStatsEffect scaledEnchant:
                    ApplyScaledEnchantStats(context, scaledEnchant);
                    break;
                case DynamicStatChangeEffect dynamicStat:
                    ApplyDynamicStatChange(context, dynamicStat);
                    break;
                case DrainDivineShieldsBuffEffect drainShields:
                    DrainDivineShieldsBuff(context, drainShields);
                    break;
                case ConsumeAdjacentMinionsEffect consume:
                    ConsumeAdjacentMinions(context, consume);
                    break;
                case SilenceEffect silence:
                    Silence(work, context, silence);
                    break;
                case RemoveAllStealthEffect:
                    RemoveAllStealth();
                    break;
                case DestroySecretsEffect destroySecrets:
                    DestroySecrets(context, destroySecrets.TargetPlayer);
                    break;
                case StealRandomEnemySecretEffect:
                    StealRandomEnemySecret(context);
                    break;
                case GrantRandomKeywordToOtherFriendlyMinionsEffect grantRandom:
                    var allies = _rules.BoardMinions(_state, context.Controller)
                        .Where(id => id != context.Source && IsLivingCharacter(id))
                        .OrderBy(id => _state.Entities[id].PlayOrder)
                        .ToArray();
                    foreach (var allyId in allies)
                    {
                        var tag = grantRandom.Tags[ChooseIndex(grantRandom.Tags.Length)];
                        var allyEntity = _state.Entities[allyId];
                        if (_rules.HasEffectiveTag(allyEntity, tag) || (tag == GAME_TAG.DIVINE_SHIELD && allyEntity.Character!.DivineShield))
                            continue;
                        var ch = allyEntity.Character!;
                        if (tag == GAME_TAG.DIVINE_SHIELD)
                            ch = ch with { DivineShield = true };
                        var layer = new StatEnchantment(context.Source, null) { GrantedTag = tag };
                        _state = _state.UpdateEntity(RecalculateStats(allyEntity with
                        {
                            Character = ch, Enchantments = allyEntity.Enchantments.Add(layer)
                        }, allyEntity.AuraGrants, !allyEntity.Silenced && _rules.Card(allyEntity).AttackEqualsHealth));
                        _events.Add(new(SimulationEventKind.EnchantmentApplied, context.Controller, context.Source, allyId));
                    }
                    break;
                case SwapHealthEffect:
                    SwapHealth(context);
                    break;
                case TriggerFriendlyDeathrattlesEffect:
                    var friendlyDeathrattles = _rules.BoardMinions(_state, context.Controller)
                        .Select(id => _state.Entities[id])
                        .Where(e => IsLivingCharacter(e.Id) && !e.Silenced &&
                            (!_rules.Card(e).Deathrattle.IsEmpty || e.Enchantments.Any(l => l.GrantedTag == GAME_TAG.DEATHRATTLE)))
                        .OrderBy(e => e.PlayOrder)
                        .ToArray();
                    var drTimes = _rules.BoardMinions(_state, context.Controller).Any(id =>
                        !_state.Entities[id].Silenced && _rules.Card(_state.Entities[id]).DoubleFriendlyDeathrattles) ? 2 : 1;
                    var drQueue = new List<Activate>();
                    foreach (var minionEntity in friendlyDeathrattles)
                    {
                        var pos = _rules.BoardSlots(_state, minionEntity.Controller).TakeWhile(slot => slot != minionEntity.Id).Count();
                        var drEffects = _rules.Card(minionEntity).Deathrattle;
                        if (!drEffects.IsEmpty)
                            for (var pass = 0; pass < drTimes; pass++)
                                drQueue.Add(new(new(minionEntity.Id, minionEntity.Controller, DeathPosition: pos), minionEntity.PlayOrder!.Value, drEffects));
                        foreach (var layer in minionEntity.Enchantments)
                            if (layer.GrantedTag == GAME_TAG.DEATHRATTLE && layer.CardId is { } enchId &&
                                _rules._enchantmentRemovalEffects.TryGetValue((enchId, EnchantmentRemovalReason.Died), out var granted))
                                for (var pass = 0; pass < drTimes; pass++)
                                    drQueue.Add(new(new(minionEntity.Id, minionEntity.Controller, Subject: minionEntity.Id, DeathPosition: pos), minionEntity.PlayOrder!.Value, granted));
                    }
                    RunQueue(drQueue);
                    break;
                case ShuffleMinionIntoDeckEffect shuffleMinion:
                    ShuffleMinionIntoDeck(work, context, shuffleMinion);
                    break;
                case MimironCombineEffect mimiron:
                    MimironCombine(work, context, mimiron);
                    break;
                case ScheduleDormantAwakenEffect awaken:
                    ScheduleDormantAwaken(context, awaken);
                    break;
                case SummonEffect summon:
                    var summonOwner = summon.Recipient == CardRecipient.Controller
                        ? context.Controller
                        : _state.OpponentOf(context.Controller);
                    var position = summon.Recipient == CardRecipient.Controller
                        ? ResolveSummonPosition(context, summon.Placement)
                        : null;
                    if (Summon(summonOwner, summon.CardId, position) is EntityId summoned)
                        PushSummonTriggers(work, summonOwner, summoned);
                    break;
                case SummonPerEnemyMinionEffect perEnemy:
                    var enemyMinionCount = _rules.BoardMinions(_state, _state.OpponentOf(context.Controller))
                        .Count(id => IsLivingCharacter(id));
                    for (var i = 0; i < enemyMinionCount; i++)
                        work.Push(new ApplyEffect(context, new SummonEffect(perEnemy.CardId)));
                    break;
                case SwapSelfWithRandomHandMinionEffect:
                    SwapSelfWithRandomHandMinion(work, context);
                    break;
                case FillBoardSummonEffect fill:
                    ApplyFillBoardSummon(work, context, fill);
                    break;
                case SummonCopyEffect copy:
                    SummonCopy(work, context, copy);
                    break;
                case CopyMinionToSelfEffect:
                    CopyMinionToSelf(context);
                    break;
                case TakeControlEffect takeControl:
                    var targetMinion = (takeControl.Target == EffectTarget.Selected ? context.Selected : context.Subject)!.Value;
                    var destination = takeControl.ReturnToSourceOpponent
                        ? _state.OpponentOf(_state.Entities[context.Source].Controller)
                        : context.Controller;
                    TakeControl(context.Source, targetMinion, destination, takeControl.ReadyToAttack);
                    break;
                case TakeControlRandomEnemyMinionEffect:
                    TakeControlRandomEnemyMinion(context);
                    break;
                case ReturnMinionEffect returned:
                    ReturnMinion(work, context, returned);
                    break;
                case TransformMinionEffect transform:
                    TransformMinion(work, context, transform.CardId, transform.Self);
                    break;
                case RandomTransformOtherMinionEffect randomTransform:
                    RandomTransformOtherMinion(work, context, randomTransform);
                    break;
                case TransformMinionByCostEffect transform:
                    TransformMinionByCost(work, context, transform.Increase);
                    break;
                case DestroyMinionEffect destroy:
                    DestroyMinion((destroy.Target == EffectTarget.Selected ? context.Selected : context.Subject)!.Value);
                    break;
                case DestroyAllMinionsEffect destroyAll:
                    DestroyAllMinions(work, context, destroyAll);
                    break;
                case ResurrectFriendlyMinionEffect:
                    ResurrectFriendlyMinion(work, context.Controller);
                    break;
                case ResurrectDiedThisTurnEffect:
                    ResurrectDiedThisTurn(work, context);
                    break;
                case ResummonMinionEffect resummon:
                    ResummonMinion(work, context, resummon);
                    break;
                case ReincarnateMinionEffect reincarnate:
                    ReincarnateMinion(work, context, reincarnate);
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
                case RedirectSpellTargetEffect redirectSpell:
                    RedirectSpellTarget(work, context, redirectSpell.CardId);
                    break;
                case PreventDamageEffect:
                    PreventDamage();
                    break;
                case SummonAttackDefenderEffect redirect:
                    SummonAttackDefender(work, context, redirect.CardId);
                    break;
                case RedirectAttackToRandomOtherCharacterEffect:
                    RedirectAttackToRandomOtherCharacter(context);
                    break;
                case RandomDamageEffect random:
                    ApplyRandomDamage(work, context, random);
                    break;
                case RandomDamageRangeEffect randomRange:
                    ApplyRandomDamageRange(work, context, randomRange);
                    break;
                case DestroyRandomEnemyMinionEffect destroyRandom:
                    DestroyRandomEnemyMinion(context, destroyRandom);
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
                case RandomChanceEffect chance:
                    if (ChooseIndex(chance.Denominator) < chance.Numerator)
                        PushEffects(work, context, chance.Then);
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
                    var deathrattleTimes = position is not null &&
                        _rules.BoardMinions(_state, entity.Controller).Any(id =>
                        {
                            var ally = _state.Entities[id];
                            return !ally.Silenced && _rules.Card(ally).DoubleFriendlyDeathrattles;
                        }) ? 2 : 1;
                    var effects = entity.Silenced ? [] : _rules.Card(entity).Deathrattle;
                    if (!effects.IsEmpty)
                        for (var pass = 0; pass < deathrattleTimes; pass++)
                            queue = queue.Append(new(new(entity.Id, entity.Controller, DeathPosition: position),
                                entity.PlayOrder!.Value, effects));
                    foreach (var layer in entity.Enchantments)
                        if (layer.GrantedTag == GAME_TAG.DEATHRATTLE && layer.CardId is { } enchantment &&
                            _rules._enchantmentRemovalEffects.TryGetValue((enchantment, EnchantmentRemovalReason.Died), out var granted))
                            for (var pass = 0; pass < deathrattleTimes; pass++)
                                queue = queue.Append(new(new(entity.Id, entity.Controller, Subject: entity.Id, DeathPosition: position),
                                    entity.PlayOrder!.Value, granted));
                    if (position is not null)
                    {
                        var awakenWork = new Stack<Work>();
                        ProcessPendingAwakens(awakenWork, entity.Controller, entity);
                        RunWork(awakenWork);
                    }
                    RunQueue(queue.OrderBy(trigger => trigger.PlayOrder));
                }
            }
        }

        private void ApplyRandomDamageRange(Stack<Work> work, EffectContext context, RandomDamageRangeEffect effect)
        {
            if (effect.RepeatUntilDeath)
            {
                var candidates = DamageCandidates(context, effect.Pool!.Value).ToArray();
                if (candidates.Length == 0)
                    return;
                SpendResolutionStep();
                var chosen = candidates[ChooseIndex(candidates.Length)];
                var amt = ModifiedEffectAmount(context, effect.MinAmount, damage: true);
                var hit = Damage(context.Source, chosen, amt);
                if (IsLivingCharacter(chosen) && _state.Entities[chosen].Character is { Health: > 0 })
                    work.Push(new ApplyEffect(context, effect));
                PushOccurrence(work, hit);
                return;
            }

            var rawAmount = effect.MinAmount + ChooseIndex(effect.MaxAmount - effect.MinAmount + 1);
            var finalAmount = ModifiedEffectAmount(context, rawAmount, damage: true);
            EntityId targetId;
            if (effect.Pool is { } pool)
            {
                var candidates = DamageCandidates(context, pool).ToArray();
                if (candidates.Length == 0)
                    return;
                targetId = candidates[ChooseIndex(candidates.Length)];
            }
            else
            {
                targetId = effect.Target switch
                {
                    DamageTarget.Selected => context.Selected!.Value,
                    DamageTarget.Source => context.Source,
                    DamageTarget.TriggerSubject => context.Subject!.Value,
                    DamageTarget.FriendlyHero => _state.Players[context.Controller].Hero,
                    DamageTarget.EnemyHero => _state.Players[_state.OpponentOf(context.Controller)].Hero,
                    _ => throw new InvalidOperationException("无法识别随机区间伤害的目标。")
                };
            }
            var occurrence = Damage(context.Source, targetId, finalAmount);
            var dealt = occurrence?.Amount ?? 0;
            if (effect.SummonPerDamageCardId is { } summonCardId && dealt > 0)
            {
                for (var i = 0; i < dealt; i++)
                {
                    if (_rules.BoardSlotCount(_state, context.Controller) >= BoardLimit)
                        break;
                    if (Summon(context.Controller, summonCardId, position: null) is EntityId summoned)
                        PushSummonTriggers(work, context.Controller, summoned);
                }
            }
            PushOccurrence(work, occurrence);
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
