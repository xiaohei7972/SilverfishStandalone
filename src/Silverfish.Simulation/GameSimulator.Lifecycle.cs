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
            var id = (effect.Target == EffectTarget.Selected ? context.Selected : context.Subject)!.Value;
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

        private void TransformMinion(Stack<Work> work, EffectContext context, string cardId)
        {
            var id = context.Selected!.Value;
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
            _events.Add(new(SimulationEventKind.Transformed, entity.Controller, context.Source, id));
            PushEnchantmentRemovalEffects(work, entity, entity.Enchantments, EnchantmentRemovalReason.Transformed);
        }

        private void DestroyMinion(EntityId id)
        {
            if (IsLivingCharacter(id))
                _pendingDestruction.Add(id);
        }

        private void DestroyAllMinions(EffectContext context, DestroyAllMinionsEffect effect)
        {
            var targets = _state.Players.Keys
                .SelectMany(player => effect.IncludeLocations ? _rules.BoardSlots(_state, player) : _rules.BoardMinions(_state, player))
                .Where(id => (!effect.ExcludeSource || id != context.Source) &&
                    (_state.Entities[id].Character is not null
                        ? IsLivingCharacter(id)
                        : _state.Entities[id].Location is { Health: > 0 }))
                .OrderBy(id => _state.Entities[id].PlayOrder)
                .ToArray();
            foreach (var id in targets)
                _pendingDestruction.Add(id);
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
