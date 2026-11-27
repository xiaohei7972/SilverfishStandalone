using System.Collections.Immutable;
using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private sealed partial class Resolution
    {
        private readonly HashSet<EntityId> _pendingDestruction = [];
        private readonly List<Death> _immediateDeaths = [];

        private sealed record Death(GameEntity Entity, int? BoardPosition)
        {
            // The standard death wave takes one observer snapshot. Immediate
            // removals instead retain the observers present when they died.
            public ImmutableArray<Listener>? ListenersAtDeath { get; init; }
        }

        private void ReturnMinion(Stack<Work> work, EffectContext context, ReturnMinionEffect effect)
        {
            EntityId id;
            if (effect.RandomFriendly)
            {
                var candidates = _rules.BoardMinions(_state, context.Controller)
                    .Where(candidate => candidate != context.Source && IsLivingCharacter(candidate))
                    .OrderBy(candidate => _state.Entities[candidate].PlayOrder)
                    .ToArray();
                if (candidates.Length == 0)
                    return;
                id = candidates[ChooseIndex(candidates.Length)];
            }
            else
            {
                id = (effect.Target == EffectTarget.Selected ? context.Selected : context.Subject)!.Value;
            }
            if (!IsLivingCharacter(id) || _rules.Card(_state.Entities[id]).Definition.Kind != EntityKind.Minion)
                return;

            var entity = _state.Entities[id];
            var controller = entity.Controller;
            if (_state.Players[controller].In(GameZone.Hand).Length == HandLimit)
            {
                RemoveImmediately(entity);
                return;
            }

            _state = _state.Move(id, new(controller, GameZone.Hand));
            var modifiers = effect.CostDelta == 0 ? entity.CostModifiers :
                entity.CostModifiers.Add(CreateCostModifier(context.Source, effect.CostDelta, effect.EnchantmentCardId, effect.CostDuration));
            _state = _state.UpdateEntity(_state.Entities[id] with
            {
                Character = null, Silenced = false, CostModifiers = modifiers
            });
            // The next effect in this same work stack must no longer use a
            // health/attack aura emitted by a minion that has left play.
            _state = _rules.RefreshAuras(_state);
            _events.Add(new(SimulationEventKind.ReturnedToHand, controller, context.Source, id) { EnchantmentCardId = effect.EnchantmentCardId });
            PushEnchantmentRemovalEffects(work, entity, entity.Enchantments, EnchantmentRemovalReason.LeftPlay);
        }

        private void TransformMinion(Stack<Work> work, EffectContext context, string cardId, bool self = false) =>
            TransformMinionId(work, context.Source, self ? context.Source : context.Selected!.Value, cardId);

        private void TransformMinionId(Stack<Work> work, EntityId source, EntityId id, string cardId)
        {
            if (!IsLivingCharacter(id))
                return;

            var entity = _state.Entities[id];
            _state = _state.UpdateEntity(entity with
            {
                CardId = cardId, Character = _rules.GetCard(cardId).CreateCharacter(), Silenced = false, Stealthed = null,
                Enchantments = [], AuraGrants = []
            });
            // Transform can remove the source card's aura before a later
            // effect in the same resolution applies damage.
            _state = _rules.RefreshAuras(_state);
            _events.Add(new(SimulationEventKind.Transformed, entity.Controller, source, id));
            PushEnchantmentRemovalEffects(work, entity, entity.Enchantments, EnchantmentRemovalReason.Transformed);
        }

        private void RandomTransformOtherMinion(Stack<Work> work, EffectContext context, RandomTransformOtherMinionEffect effect)
        {
            var candidates = _state.Players.Keys.SelectMany(player => _rules.BoardMinions(_state, player))
                .Where(id => id != context.Source && IsLivingCharacter(id))
                .OrderBy(id => _state.Entities[id].PlayOrder)
                .ToArray();
            if (candidates.Length == 0)
                return;

            var targetId = candidates[ChooseIndex(candidates.Length)];
            var cardId = effect.CardIds[ChooseIndex(effect.CardIds.Length)];
            TransformMinionId(work, context.Source, targetId, cardId);
        }

        private void CopyMinionToSelf(EffectContext context)
        {
            var targetId = context.Selected!.Value;
            if (!IsLivingCharacter(targetId) || !IsLivingCharacter(context.Source))
                return;

            var original = _state.Entities[targetId];
            var self = _state.Entities[context.Source];
            if (_rules.Card(original).Definition.Kind != EntityKind.Minion || original.Character is not { } character)
                return;

            var copied = self with
            {
                CardId = original.CardId,
                Stealthed = original.Stealthed,
                Silenced = original.Silenced,
                Enchantments = [.. original.Enchantments.Where(layer => layer.GrantedTag != GAME_TAG.CONTROLLER)],
                AuraGrants = [],
                Character = character with { AttacksThisTurn = 0, Exhausted = true }
            };
            copied = RecalculateStats(copied, []);
            _state = _state.UpdateEntity(copied);
            _state = _rules.RefreshAuras(_state);
            _events.Add(new(SimulationEventKind.Transformed, self.Controller, context.Source, self.Id));
        }

        private void SetHeroRemainingHealth(EffectContext context, SetHeroRemainingHealthEffect effect)
        {
            var targetId = (effect.Target == EffectTarget.Selected ? context.Selected : context.Subject)!.Value;
            if (!IsLivingCharacter(targetId))
                return;

            var entity = _state.Entities[targetId];
            if (_rules.Card(entity).Definition.Kind != EntityKind.Hero || entity.Character is not { } character)
                return;

            var oldHealth = character.Health;
            var updated = character.MaximumHealth < effect.Health
                ? character with { MaximumHealth = effect.Health, Damage = 0 }
                : character with { Damage = character.MaximumHealth - effect.Health };
            _state = _state.UpdateCharacter(targetId, updated);
            if (updated.Health != oldHealth)
            {
                var owner = _state.Players[entity.Controller];
                _state = _state.UpdateResources(owner with { HeroHealthChangedThisTurn = true });
                _state = _rules.RefreshAuras(_state);
            }
            _events.Add(new(SimulationEventKind.EnchantmentApplied, context.Controller, context.Source, targetId));
        }

        private void DestroyMinion(EntityId id)
        {
            if (IsLivingCharacter(id))
                _pendingDestruction.Add(id);
        }

        private void TakeControlRandomEnemyMinion(EffectContext context)
        {
            var enemy = _state.OpponentOf(context.Controller);
            var candidates = _rules.BoardMinions(_state, enemy)
                .Where(id => IsLivingCharacter(id))
                .OrderBy(id => _state.Entities[id].PlayOrder)
                .ToArray();
            if (candidates.Length > 0)
                TakeControl(context.Source, candidates[ChooseIndex(candidates.Length)], context.Controller);
        }

        private void DrainDivineShieldsBuff(EffectContext context, DrainDivineShieldsBuffEffect effect)
        {
            var minions = _state.Players.Keys.SelectMany(player => _rules.BoardMinions(_state, player))
                .Where(id => IsLivingCharacter(id))
                .OrderBy(id => _state.Entities[id].PlayOrder)
                .ToArray();
            var shieldsLost = 0;
            foreach (var id in minions)
            {
                var entity = _state.Entities[id];
                if (entity.Character is not { DivineShield: true } character)
                    continue;

                shieldsLost++;
                _state = _state.UpdateEntity(entity with
                {
                    Character = character with { DivineShield = false },
                    Enchantments = [.. entity.Enchantments.Where(layer => layer.GrantedTag != GAME_TAG.DIVINE_SHIELD)]
                });
                _events.Add(new(SimulationEventKind.ShieldBroken, entity.Controller, context.Source, id));
            }
            if (shieldsLost > 0 && IsLivingCharacter(context.Source))
            {
                int? atk = effect.AttackPerShield != 0 ? checked(effect.AttackPerShield * shieldsLost) : null;
                int? hp = effect.HealthPerShield != 0 ? checked(effect.HealthPerShield * shieldsLost) : null;
                EnchantStats(context, new(StatTarget.Source, new(StatModifierMode.Add, atk, hp))
                {
                    EnchantmentCardId = effect.EnchantmentCardId
                });
            }
        }

        private void ConsumeAdjacentMinions(EffectContext context, ConsumeAdjacentMinionsEffect effect)
        {
            if (!IsLivingCharacter(context.Source))
                return;

            var adjacent = AdjacentMinions(context.Source);
            if (adjacent.IsEmpty)
                return;

            var totalAttack = 0;
            var totalHealth = 0;
            foreach (var id in adjacent)
            {
                var entity = _state.Entities[id];
                totalAttack = checked(totalAttack + _rules.EffectiveAttack(_state, entity));
                totalHealth = checked(totalHealth + Math.Max(0, entity.Character!.Health));
                DestroyMinion(id);
            }
            if (totalAttack > 0 || totalHealth > 0)
            {
                EnchantStats(context, new(StatTarget.Source,
                    new(StatModifierMode.Add, totalAttack > 0 ? totalAttack : null, totalHealth > 0 ? totalHealth : null))
                {
                    EnchantmentCardId = effect.EnchantmentCardId
                });
            }
        }

        private void SwapSelfWithRandomHandMinion(Stack<Work> work, EffectContext context)
        {
            if (!IsLivingCharacter(context.Source))
                return;

            var controller = context.Controller;
            var candidates = _state.Players[controller].In(GameZone.Hand)
                .Where(id => _rules.Card(_state.Entities[id]).Definition.Kind == EntityKind.Minion)
                .ToArray();
            if (candidates.Length == 0)
                return;

            var chosen = candidates[ChooseIndex(candidates.Length)];
            var slot = _rules.BoardSlots(_state, controller).TakeWhile(id => id != context.Source).Count();
            var selfEntity = _state.Entities[context.Source];
            _state = _state.Move(context.Source, new(controller, GameZone.Hand));
            _state = _state.UpdateEntity(_state.Entities[context.Source] with { Character = null, Silenced = false });
            _events.Add(new(SimulationEventKind.ReturnedToHand, controller, context.Source, context.Source));
            PushEnchantmentRemovalEffects(work, selfEntity, selfEntity.Enchantments, EnchantmentRemovalReason.LeftPlay);

            var card = _rules.Card(_state.Entities[chosen]);
            var index = _rules.BoardIndex(_state, controller, slot);
            _state = _state.Move(chosen, new(controller, GameZone.Play), index);
            _state = _state.UpdateCharacter(chosen, card.CreateCharacter());
            FinishSummon(controller, chosen);
            PushSummonTriggers(work, controller, chosen);
        }

        private void DestroyAllMinions(Stack<Work> work, EffectContext context, DestroyAllMinionsEffect effect)
        {
            var targets = _state.Players.Keys
                .SelectMany(player => effect.IncludeLocations ? _rules.BoardSlots(_state, player) : _rules.BoardMinions(_state, player))
                .Where(id => (!effect.ExcludeSource || id != context.Source) &&
                    (_state.Entities[id].Character is not null
                        ? IsLivingCharacter(id)
                        : _state.Entities[id].Location is { Health: > 0 }))
                .OrderBy(id => _state.Entities[id].PlayOrder)
                .ToArray();
            Dictionary<PlayerId, int>? destroyedCounts = effect.ReplaceWithCardId is null ? null : [];
            foreach (var id in targets)
            {
                if (destroyedCounts is not null)
                {
                    var owner = _state.Entities[id].Controller;
                    destroyedCounts[owner] = destroyedCounts.GetValueOrDefault(owner) + 1;
                }
                _pendingDestruction.Add(id);
            }
            if (effect.ReplaceWithCardId is not { } replaceId || destroyedCounts is null || destroyedCounts.Count == 0)
                return;

            ResolveDeaths();
            foreach (var player in _state.Players.Keys)
            {
                var count = destroyedCounts.GetValueOrDefault(player);
                for (var i = 0; i < count; i++)
                {
                    if (_rules.BoardSlotCount(_state, player) >= BoardLimit)
                        break;
                    if (Summon(player, replaceId, position: null) is EntityId summoned)
                        PushSummonTriggers(work, player, summoned);
                }
            }
        }

        private void ResurrectDiedThisTurn(Stack<Work> work, EffectContext context)
        {
            var player = context.Controller;
            var diedThisTurn = _state.MinionDeaths
                .Where(death => death.Controller == player && death.Turn == _state.Turn)
                .ToArray();
            foreach (var death in diedThisTurn)
            {
                if (_rules.BoardSlotCount(_state, player) >= BoardLimit)
                    break;
                if (Summon(player, death.CardId, position: null) is EntityId summoned)
                {
                    _events.Add(new(SimulationEventKind.Resurrected, player, death.Entity, summoned));
                    PushSummonTriggers(work, player, summoned);
                }
            }
        }

        private void SummonFromHand(Stack<Work> work, EffectContext context, SummonFromHandEffect effect)
        {
            var player = ResolveRecipient(effect.Recipient, context.Controller);
            for (var i = 0; i < effect.Count; i++)
            {
                if (_rules.BoardSlotCount(_state, player) >= BoardLimit)
                    break;
                var candidates = _state.Players[player].In(GameZone.Hand)
                    .Where(id =>
                    {
                        var card = _rules.Card(_state.Entities[id]);
                        return card.Definition.Kind == EntityKind.Minion &&
                            (effect.CardFilter is null || MatchesCardFilter(card, effect.CardFilter));
                    }).ToArray();
                if (candidates.Length == 0)
                    break;

                SpendResolutionStep();
                var chosen = candidates[ChooseIndex(candidates.Length)];
                SummonExistingMinion(player, chosen);
                PushSummonTriggers(work, player, chosen);
            }
        }

        private void ReincarnateMinion(Stack<Work> work, EffectContext context, ReincarnateMinionEffect effect)
        {
            var id = (effect.Target == EffectTarget.Selected ? context.Selected : context.Subject)!.Value;
            if (!IsLivingCharacter(id) || _rules.Card(_state.Entities[id]).Definition.Kind != EntityKind.Minion)
                return;

            var entity = _state.Entities[id];
            var owner = entity.Controller;
            var cardId = entity.CardId;
            var position = _rules.BoardSlots(_state, owner).TakeWhile(slot => slot != id).Count();
            RemoveImmediately(entity);
            ResolveDeaths();
            if (_rules.BoardSlotCount(_state, owner) >= BoardLimit)
                return;
            if (Summon(owner, cardId, position) is EntityId summoned)
            {
                _events.Add(new(SimulationEventKind.Resurrected, owner, id, summoned));
                PushSummonTriggers(work, owner, summoned);
            }
        }

        private void SwapHealth(EffectContext context)
        {
            var targetId = context.Selected!.Value;
            if (!IsLivingCharacter(context.Source) || !IsLivingCharacter(targetId))
                return;

            var source = _state.Entities[context.Source];
            var target = _state.Entities[targetId];
            if (source.Character is not { } sourceChar || target.Character is not { } targetChar)
                return;

            var sourceHealth = Math.Max(0, sourceChar.Health);
            var targetHealth = Math.Max(0, targetChar.Health);
            EnchantStats(context, new(StatTarget.Source, new(StatModifierMode.Set, MaximumHealth: targetHealth)));
            EnchantStats(context, new(StatTarget.Selected, new(StatModifierMode.Set, MaximumHealth: sourceHealth)));
        }

        private void ShuffleMinionIntoDeck(Stack<Work> work, EffectContext context, ShuffleMinionIntoDeckEffect effect)
        {
            var id = (effect.Target == EffectTarget.Selected ? context.Selected : context.Subject)!.Value;
            if (!IsLivingCharacter(id) || _rules.Card(_state.Entities[id]).Definition.Kind != EntityKind.Minion)
                return;

            var entity = _state.Entities[id];
            var owner = entity.Controller;
            _state = _state.Move(id, new(owner, GameZone.Deck));
            _state = _state.UpdateEntity(_state.Entities[id] with
            {
                Character = null, Silenced = false, Stealthed = null, Enchantments = [], AuraGrants = []
            });
            _state = _rules.RefreshAuras(_state);
            _events.Add(new(SimulationEventKind.CardAddedToDeck, owner, context.Source, id));
            PushEnchantmentRemovalEffects(work, entity, entity.Enchantments, EnchantmentRemovalReason.LeftPlay);
            ShuffleDeck(owner, context.Source);
        }

        private void StealRandomEnemySecret(EffectContext context)
        {
            var controller = context.Controller;
            var enemy = _state.OpponentOf(controller);
            var candidates = _state.Players[enemy].In(GameZone.Secret)
                .Where(id => CanArmSecret(_state, _state.Players[controller], _state.Entities[id].CardId))
                .ToArray();
            if (candidates.Length == 0)
                return;

            var chosen = candidates[ChooseIndex(candidates.Length)];
            _state = _state.Move(chosen, new(controller, GameZone.Secret));
            _events.Add(new(SimulationEventKind.ControlChanged, controller, context.Source, chosen));
            _events.Add(new(SimulationEventKind.SecretArmed, controller, chosen));
        }

        private void MimironCombine(Stack<Work> work, EffectContext context, MimironCombineEffect effect)
        {
            if (!IsLivingCharacter(context.Source))
                return;

            var controller = context.Controller;
            var mechs = _rules.BoardMinions(_state, controller)
                .Where(id => IsLivingCharacter(id) && MatchesCardFilter(_rules.Card(_state.Entities[id]), new() { MinionType = MinionType.Mech }))
                .OrderBy(id => _state.Entities[id].PlayOrder)
                .ToArray();
            if (mechs.Length < effect.MinimumMechs)
                return;

            foreach (var id in mechs)
                RemoveImmediately(_state.Entities[id]);
            ResolveDeaths();
            if (_rules.BoardSlotCount(_state, controller) < BoardLimit && Summon(controller, effect.TokenCardId, position: null) is EntityId summoned)
                PushSummonTriggers(work, controller, summoned);
        }

        private void ScheduleDormantAwaken(EffectContext context, ScheduleDormantAwakenEffect effect)
        {
            var player = _state.Players[context.Controller];
            var entry = new PendingAwaken(effect.CardId, effect.RequiredDeaths, effect.RequiredMinionType);
            _state = _state.UpdateResources(player with { PendingAwakens = player.PendingAwakens.Add(entry) });
        }

        private void ProcessPendingAwakens(Stack<Work> work, PlayerId controller, GameEntity deadMinion)
        {
            var player = _state.Players[controller];
            if (player.PendingAwakens.IsEmpty)
                return;

            var deadDef = _rules.Card(deadMinion).Definition;
            var remaining = ImmutableArray.CreateBuilder<PendingAwaken>();
            var awakened = new List<string>();
            foreach (var awaken in player.PendingAwakens)
            {
                if (awaken.RequiredMinionType is MinionType race && !MatchesMinionType(deadDef, race))
                {
                    remaining.Add(awaken);
                    continue;
                }
                var next = awaken.Remaining - 1;
                if (next <= 0)
                    awakened.Add(awaken.CardId);
                else
                    remaining.Add(awaken with { Remaining = next });
            }

            _state = _state.UpdateResources(player with { PendingAwakens = remaining.ToImmutable() });
            foreach (var cardId in awakened)
            {
                if (_rules.BoardSlotCount(_state, controller) >= BoardLimit)
                    break;
                if (Summon(controller, cardId, position: null) is EntityId summoned)
                    PushSummonTriggers(work, controller, summoned);
            }
        }

        private void ResurrectFriendlyMinion(Stack<Work> work, PlayerId player)
        {
            if (_rules.BoardSlotCount(_state, player) >= BoardLimit)
                return;

            var candidates = _state.MinionDeaths.Where(death => death.Controller == player).ToArray();
            if (candidates.Length == 0)
                return;

            var death = candidates[ChooseIndex(candidates.Length)];
            if (Summon(player, death.CardId, position: null) is EntityId summoned)
            {
                _events.Add(new(SimulationEventKind.Resurrected, player, death.Entity, summoned));
                PushSummonTriggers(work, player, summoned);
            }
        }

        private void ResummonMinion(Stack<Work> work, EffectContext context, ResummonMinionEffect effect)
        {
            var id = (effect.Target == EffectTarget.Selected ? context.Selected : (context.Subject ?? context.Source))!.Value;
            if (!_state.Entities.TryGetValue(id, out var target) ||
                _rules.Card(target).Definition.Kind != EntityKind.Minion)
                return;

            if (Summon(context.Controller, target.CardId, context.DeathPosition, effect.RemainingHealth) is EntityId summoned)
            {
                _events.Add(new(SimulationEventKind.Resurrected, context.Controller, id, summoned));
                PushSummonTriggers(work, context.Controller, summoned);
            }
        }

        private void SummonCopy(Stack<Work> work, EffectContext context, SummonCopyEffect effect)
        {
            var player = context.Controller;
            if (_rules.BoardSlotCount(_state, player) >= BoardLimit)
                return;

            var targetId = (effect.Target == EffectTarget.Selected ? context.Selected : context.Subject)!.Value;
            if (!IsLivingCharacter(targetId))
                return;

            var original = _state.Entities[targetId];
            if (_rules.Card(original).Definition.Kind != EntityKind.Minion || original.Character is not { } character)
                return;

            var id = new EntityId(_state.NextEntity);
            var copy = new GameEntity(id, original.CardId, player, player)
            {
                Stealthed = original.Stealthed,
                Silenced = original.Silenced,
                Enchantments = [.. original.Enchantments.Where(layer => layer.GrantedTag != GAME_TAG.CONTROLLER)],
                AuraGrants = original.AuraGrants,
                Character = character with { AttacksThisTurn = 0, Exhausted = true }
            };
            copy = RecalculateStats(copy, []);
            _state = _state.AddToPlay(copy, _state.Players[player].In(GameZone.Play).Length);
            FinishSummon(player, id);
            PushSummonTriggers(work, player, id);
        }

        private static void PushSummonTriggers(Stack<Work> work, PlayerId player, EntityId summoned)
        {
            work.Push(new Dispatch(new(TriggerTiming.AfterMinionSummoned, player, summoned)));
            work.Push(new Dispatch(new(TriggerTiming.WhenMinionSummoned, player, summoned)));
        }

        // Full-hand bounces and board-overflow control changes remove a minion
        // before the regular death wave. Preserve the pre-removal observers and
        // update their former aura recipients before the next queued effect.
        private void RemoveImmediately(GameEntity entity)
        {
            var listeners = CaptureListeners();
            _immediateDeaths.Add(RemoveDeadEntity(entity) with { ListenersAtDeath = listeners });
            _state = _rules.RefreshAuras(_state);
        }

        private Death RemoveDeadEntity(GameEntity entity)
        {
            SpendResolutionStep();
            int? position = null;
            var card = _rules.Card(entity);
            if (card.Definition.Kind == EntityKind.Minion)
            {
                position = _rules.BoardSlots(_state, entity.Controller).TakeWhile(id => id != entity.Id).Count();
                _state = _state.RecordMinionDeath(entity, !card.LeavesNoCorpse);
            }

            if (!entity.AuraGrants.IsEmpty || !entity.Enchantments.IsEmpty)
                _state = _state.UpdateEntity(RecalculateStats(entity with { Enchantments = [] }, []));
            if (entity.Location is not null)
                _state = _state.UpdateEntity(_state.Entities[entity.Id] with { Location = new(0) });
            _state = _state.Move(entity.Id, new(entity.Controller, GameZone.Graveyard));
            _pendingDestruction.Remove(entity.Id);
            var kind = entity.Weapon is not null
                ? SimulationEventKind.WeaponDestroyed
                : entity.Location is not null ? SimulationEventKind.LocationDestroyed : SimulationEventKind.Died;
            _events.Add(new(kind, entity.Controller, entity.Id));
            return new(entity, position);
        }
    }
}
