using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private sealed partial class Resolution
    {
        private readonly HashSet<EntityId> _pendingDestruction = [];
        private readonly List<Death> _immediateDeaths = [];

        private sealed record Death(GameEntity Entity, int? MinionPosition);

        private void ReturnMinion(EffectContext context, ReturnMinionEffect effect)
        {
            var id = context.Selected!.Value;
            if (!IsLivingCharacter(id))
                return;

            var entity = _state.Entities[id];
            var controller = entity.Controller;
            if (_state.Players[controller].In(GameZone.Hand).Length == HandLimit)
            {
                _immediateDeaths.Add(RemoveDeadEntity(entity));
                return;
            }

            _state = _state.Move(id, new(controller, GameZone.Hand));
            var modifiers = effect.CostDelta == 0
                ? entity.CostModifiers : entity.CostModifiers.Add(new(context.Source, effect.CostDelta));
            _state = _state.UpdateEntity(_state.Entities[id] with
            {
                Character = null, Silenced = false, CostModifiers = modifiers
            });
            _events.Add(new(SimulationEventKind.ReturnedToHand, controller, context.Source, id));
        }

        private void TransformMinion(EffectContext context, string cardId)
        {
            var id = context.Selected!.Value;
            if (!IsLivingCharacter(id))
                return;

            var entity = _state.Entities[id];
            _state = _state.UpdateEntity(entity with
            {
                CardId = cardId, Character = _rules.GetCard(cardId).CreateCharacter(), Silenced = false,
                Enchantments = [], AuraGrants = []
            });
            _events.Add(new(SimulationEventKind.Transformed, entity.Controller, context.Source, id));
        }

        private void DestroyMinion(EntityId id)
        {
            if (IsLivingCharacter(id))
                _pendingDestruction.Add(id);
        }

        private void ResurrectFriendlyMinion(Stack<Work> work, PlayerId player)
        {
            if (_rules.MinionCount(_state, player) >= BoardLimit)
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

        private int ChooseIndex(int count)
        {
            if (count == 1)
                return 0;

            // SplitMix64 gives each branch a reproducible stream. Reject the short tail before taking a modulus.
            var randomState = _state.RandomState;
            var bound = (ulong)count;
            var threshold = unchecked(0UL - bound) % bound;
            ulong value;
            do
            {
                randomState = unchecked(randomState + 0x9E3779B97F4A7C15UL);
                value = unchecked((randomState ^ (randomState >> 30)) * 0xBF58476D1CE4E5B9UL);
                value = unchecked((value ^ (value >> 27)) * 0x94D049BB133111EBUL);
                value ^= value >> 31;
            } while (value < threshold);

            _state = _state.SetRandomState(randomState);
            return (int)(value % bound);
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
            if (_rules.Card(entity).Definition.Kind == EntityKind.Minion)
            {
                position = _rules.BoardMinions(_state, entity.Controller).TakeWhile(id => id != entity.Id).Count();
                _state = _state.RecordMinionDeath(entity);
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
