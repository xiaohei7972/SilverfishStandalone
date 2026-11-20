using System.Collections.Immutable;
using Silverfish.Domain;

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

        public SimulationResult Run(SimulationAction action)
        {
            switch (action.Kind)
            {
                case SimulationActionKind.EndTurn:
                    EndTurn(action.Actor);
                    break;
                case SimulationActionKind.PlayCard:
                    Play(action.Actor, action.Source!.Value, action.Target, action.Position, action.Choice);
                    break;
                case SimulationActionKind.Attack:
                    Attack(action.Actor, action.Source!.Value, action.Target!.Value);
                    break;
                case SimulationActionKind.UseHeroPower:
                    UseHeroPower(action.Actor, action.Source!.Value, action.Target);
                    break;
            }

            ResolveDeaths();
            FinishIfHeroesDied();
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
            ExpireTurnEnchantments();
            var previous = _state.Players[outgoing];
            if (previous.TemporaryMana != 0)
                _state = _state.UpdateResources(previous with { TemporaryMana = 0 });
            _state = _state.PassTurn();
            var next = _state.Players[_state.ActivePlayer];
            _state = _state.UpdateResources(next with
            {
                ManaCrystals = next.ManaCrystals + Math.Min(1, next.MaximumManaCrystals - next.ManaCrystals),
                ManaSpent = 0, TemporaryMana = 0, OverloadLocked = next.OverloadOwed, OverloadOwed = 0,
                HeroPowerUsesThisTurn = 0
            });
            foreach (var id in next.In(GameZone.Play))
            {
                if (_state.Entities[id].Character is { } character)
                    _state = _state.UpdateCharacter(id, character with { AttacksThisTurn = 0, Exhausted = false });
            }

            _events.Add(new(SimulationEventKind.TurnStarted, next.Id));
            RunTriggers(new(TriggerTiming.TurnStarted, next.Id), CaptureListeners());
            ResolveDeaths();
            FinishIfHeroesDied();
            if (_state.Outcome == MatchOutcome.InProgress)
                RunWork(new Stack<Work>([new DrawCards(next.Id, 1)]));
        }

        private void Draw(Stack<Work> work, PlayerId playerId)
        {
            SpendResolutionStep();
            var player = _state.Players[playerId];
            var deck = player.In(GameZone.Deck);
            if (deck.IsEmpty)
            {
                var fatigue = checked(player.Fatigue + 1);
                _state = _state.UpdateResources(player with { Fatigue = fatigue });
                _events.Add(new(SimulationEventKind.Fatigue, playerId, Target: player.Hero, Amount: fatigue));
                PushOccurrence(work, Damage(null, player.Hero, fatigue));
                return;
            }

            var drawn = deck[0];
            var burned = player.In(GameZone.Hand).Length >= HandLimit;
            _state = _state.Move(drawn, new(playerId, burned ? GameZone.Graveyard : GameZone.Hand));
            _events.Add(new(burned ? SimulationEventKind.CardBurned : SimulationEventKind.CardDrawn, playerId, drawn));
            if (burned)
                return;

            PushTriggers(work, Queue(new(TriggerTiming.CardDrawn, playerId, drawn), CaptureListeners()));
            var drawEffects = _rules.Card(_state.Entities[drawn]).WhenDrawn;
            if (!drawEffects.IsEmpty)
            {
                _events.Add(new(SimulationEventKind.DrawnEffect, playerId, drawn));
                PushEffects(work, new(drawn, playerId), drawEffects);
            }
        }

        private void Play(PlayerId playerId, EntityId source, EntityId? target, int? position, string? choice)
        {
            var card = _rules.Card(_state.Entities[source]);
            var kind = card.Definition.Kind;
            var effects = choice is null ? card.OnPlay : card.Choices.Single(option => option.Id == choice).Effects;
            var listeners = CaptureListeners();
            PayMana(playerId, _rules.EffectiveCost(_state.Entities[source]));
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
                if (card.Target == TargetRequirement.None || target is not null)
                    ResolveEffects(new(source, playerId, target), effects);
                MarkEquippedWeaponForDestruction(playerId);
                SetEquippedWeapon(playerId, source);
                return;
            }

            var isMinion = kind == EntityKind.Minion;
            if (isMinion)
            {
                _state = _state.UpdateCharacter(source, card.CreateCharacter());
                _state = _rules.RefreshAuras(_state);
                _events.Add(new(SimulationEventKind.Summoned, playerId, source));
            }

            var before = isMinion ? TriggerTiming.WhenMinionSummoned : TriggerTiming.BeforeSpellResolved;
            RunTriggers(new(before, playerId, source), listeners);
            AddOverload(playerId, source, card.Overload);
            ResolveDeaths();

            if (card.Target == TargetRequirement.None || target is not null)
                ResolveEffects(new(source, playerId, target), effects);
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
            var retaliation = _rules.EffectiveAttack(_state, _state.Entities[target]);
            var hit = Damage(source, target, _rules.EffectiveAttack(_state, attacker));
            var counter = Damage(target, source, retaliation);

            // 先记录双方受到的伤害，再处理伤害事件的触发队列。
            var work = new Stack<Work>();
            PushOccurrence(work, counter);
            PushOccurrence(work, hit);
            RunWork(work);
            var afterDamage = _state.Entities[source].Character!;
            _state = _state.UpdateCharacter(source, afterDamage with { AttacksThisTurn = character.AttacksThisTurn + 1 });
            SpendWeaponDurability(weapon);
        }

        private Occurrence? Damage(EntityId? source, EntityId target, int amount)
        {
            if (amount == 0 || _state.LocationOf(target).Zone != GameZone.Play)
                return null;
            var entity = _state.Entities[target];
            var character = entity.Character!;
            if (character.DivineShield)
            {
                _state = _state.UpdateCharacter(target, character with { DivineShield = false });
                _events.Add(new(SimulationEventKind.ShieldBroken, entity.Controller, source, target));
                return null;
            }

            var absorbed = Math.Min(character.Armor, amount);
            _state = _state.UpdateCharacter(target, character with
            {
                Armor = character.Armor - absorbed,
                Damage = checked(character.Damage + (amount - absorbed))
            });
            _events.Add(new(SimulationEventKind.Damage, entity.Controller, source, target, amount));
            return new(TriggerTiming.Damage, entity.Controller, target,
                source is EntityId sourceId ? new(sourceId, _state.Entities[sourceId].Controller) : null, amount);
        }

        private static void PushOccurrence(Stack<Work> work, Occurrence? occurrence)
        {
            if (occurrence is { } happened)
                work.Push(new Dispatch(happened));
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

        private void ExecuteEffect(Stack<Work> work, EffectContext context, CardEffect effect)
        {
            switch (effect)
            {
                case DamageEffect { Target: DamageTarget.AllMinions or DamageTarget.EnemyMinions or DamageTarget.AllEnemies } damage:
                    var enemy = _state.OpponentOf(context.Controller);
                    var targets = damage.Target switch
                    {
                        DamageTarget.AllEnemies => _state.Players[enemy].In(GameZone.Play).Where(id => _state.Entities[id].Character is not null),
                        DamageTarget.EnemyMinions => _rules.BoardMinions(_state, enemy),
                        _ => _state.Players.Keys.SelectMany(player => _rules.BoardMinions(_state, player))
                    };
                    var hits = targets
                        .OrderBy(id => _state.Entities[id].PlayOrder)
                        .Select(id => Damage(context.Source, id, damage.Amount)).ToArray();
                    for (var index = hits.Length - 1; index >= 0; index--)
                        PushOccurrence(work, hits[index]);
                    break;
                case DamageEffect damage:
                    var target = damage.Target switch
                    {
                        DamageTarget.Selected => context.Selected!.Value,
                        DamageTarget.Source => context.Source,
                        DamageTarget.TriggerSubject => context.Subject!.Value,
                        DamageTarget.FriendlyHero => _state.Players[context.Controller].Hero,
                        DamageTarget.EnemyHero => _state.Players[_state.OpponentOf(context.Controller)].Hero,
                        _ => throw new InvalidOperationException("Unknown damage target.")
                    };
                    PushOccurrence(work, Damage(context.Source, target, damage.Amount));
                    break;
                case DrawEffect draw:
                    work.Push(new DrawCards(context.Controller, draw.Count));
                    break;
                case GainArmorEffect armor:
                    GainArmor(context, armor.Amount);
                    break;
                case HealFriendlyHeroEffect heal:
                    HealFriendlyHero(context, heal.Amount);
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
                    Silence(context);
                    break;
                case SummonEffect summon:
                    var position = summon.Placement == SummonPlacement.Rightmost ? null : context.DeathPosition;
                    if (Summon(context.Controller, summon.CardId, position) is EntityId summoned)
                        PushSummonTriggers(work, context.Controller, summoned);
                    break;
                case TakeControlEffect:
                    TakeControl(context.Source, context.Selected!.Value, context.Controller);
                    break;
                case ReturnMinionEffect returned:
                    ReturnMinion(context, returned);
                    break;
                case TransformMinionEffect transform:
                    TransformMinion(context, transform.CardId);
                    break;
                case DestroyMinionEffect:
                    DestroyMinion(context.Selected!.Value);
                    break;
                case ResurrectFriendlyMinionEffect:
                    ResurrectFriendlyMinion(work, context.Controller);
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
                case ChangeManaEffect mana:
                    ChangeMana(context, mana);
                    break;
            }
        }

        private EntityId? Summon(PlayerId player, string cardId, int? position)
        {
            if (_rules.MinionCount(_state, player) >= BoardLimit)
                return null;
            var id = new EntityId(_state.NextEntity);
            var entity = new GameEntity(id, cardId, player, player) { Character = _rules.GetCard(cardId).CreateCharacter() };
            var index = position is int slot ? _rules.BoardIndex(_state, player, slot) : _state.Players[player].In(GameZone.Play).Length;
            _state = _state.AddToPlay(entity, index);
            _state = _rules.RefreshAuras(_state);
            _events.Add(new(SimulationEventKind.Summoned, player, id));
            return id;
        }

        private void TakeControl(EntityId source, EntityId target, PlayerId controller)
        {
            var entity = _state.Entities[target];
            if (!IsLivingCharacter(target) || entity.Controller == controller)
                return;
            if (_rules.MinionCount(_state, controller) >= BoardLimit)
            {
                _immediateDeaths.Add(RemoveDeadEntity(entity));
                return;
            }
            _state = _state.Move(target, new(controller, GameZone.Play));
            _state = _state.UpdateCharacter(target, entity.Character! with { Exhausted = true });
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
                foreach (var (entity, position) in wave.OrderBy(death => death.Entity.PlayOrder))
                {
                    if (position is null && entity.Weapon is null)
                        continue;
                    var timing = position is not null ? TriggerTiming.MinionDied : TriggerTiming.WeaponDestroyed;
                    var queue = Queue(new(timing, entity.Controller, entity.Id), listeners);
                    var effects = entity.Silenced ? [] : _rules.Card(entity).Deathrattle;
                    if (!effects.IsEmpty)
                        queue = queue.Append(new(new(entity.Id, entity.Controller, DeathPosition: position),
                            entity.PlayOrder!.Value, effects));
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
                throw new InvalidOperationException("The action exceeded the simulator's resolution budget.");
        }
    }
}
