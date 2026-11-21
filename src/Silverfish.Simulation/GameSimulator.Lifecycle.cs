using System.Collections.Immutable;
using Silverfish.Domain;

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
                var listeners = CaptureListeners();
                _immediateDeaths.Add(RemoveDeadEntity(entity) with { ListenersAtDeath = listeners });
                return;
            }

            _state = _state.Move(id, new(controller, GameZone.Hand));
            var modifiers = effect.CostDelta == 0 ? entity.CostModifiers :
                entity.CostModifiers.Add(CreateCostModifier(context.Source, effect.CostDelta, effect.EnchantmentCardId, effect.CostDuration));
            _state = _state.UpdateEntity(_state.Entities[id] with
            {
                Character = null, Silenced = false, CostModifiers = modifiers
            });
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
            _events.Add(new(SimulationEventKind.Transformed, entity.Controller, context.Source, id));
            PushEnchantmentRemovalEffects(work, entity, entity.Enchantments, EnchantmentRemovalReason.Transformed);
        }

        private void DestroyMinion(EntityId id)
        {
            if (IsLivingCharacter(id))
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

        private static void PushSummonTriggers(Stack<Work> work, PlayerId player, EntityId summoned)
        {
            work.Push(new Dispatch(new(TriggerTiming.AfterMinionSummoned, player, summoned)));
            work.Push(new Dispatch(new(TriggerTiming.WhenMinionSummoned, player, summoned)));
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
            _state = _state.Move(entity.Id, new(entity.Controller, GameZone.Graveyard));
            _pendingDestruction.Remove(entity.Id);
            var kind = entity.Weapon is null ? SimulationEventKind.Died : SimulationEventKind.WeaponDestroyed;
            _events.Add(new(kind, entity.Controller, entity.Id));
            return new(entity, position);
        }
    }
}
