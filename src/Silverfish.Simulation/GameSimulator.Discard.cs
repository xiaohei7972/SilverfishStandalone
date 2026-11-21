using System.Collections.Immutable;
using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private sealed partial class Resolution
    {
        private void Discard(Stack<Work> work, EffectContext context, HandRecipient recipient, int count, DiscardSelection selection)
        {
            var player = recipient == HandRecipient.Controller ? context.Controller : _state.OpponentOf(context.Controller);
            var hand = _state.Players[player].In(GameZone.Hand);
            if (hand.IsEmpty)
                return;

            var discarded = count >= hand.Length ? hand : SelectDiscards(player, hand, count, selection);
            // 先把整批牌移入墓地，再从原手牌左侧开始处理各自的弃牌结算。
            foreach (var id in discarded)
            {
                SpendResolutionStep();
                _state = _state.Move(id, new(player, GameZone.Graveyard));
                _events.Add(new(SimulationEventKind.CardDiscarded, player, id));
            }

            var listeners = CaptureListeners();
            for (var i = discarded.Length - 1; i >= 0; i--)
            {
                var id = discarded[i];
                if (_rules.Card(_state.Entities[id]).WhenDiscarded is { IsEmpty: false } effects)
                    work.Push(new ResolveDiscardedCard(player, id, effects));
                PushTriggers(work, Queue(new(TriggerTiming.CardDiscarded, player, id), listeners));
            }
        }

        private ImmutableArray<EntityId> SelectDiscards(PlayerId player, ImmutableArray<EntityId> hand,
            int count, DiscardSelection selection)
        {
            var auras = selection == DiscardSelection.Random ? [] : _rules.ActiveCostAuras(_state, player);
            var selected = new HashSet<EntityId>();
            foreach (var group in hand.GroupBy(Priority).OrderByDescending(group => group.Key))
            {
                var tied = group.ToArray();
                var remaining = count - selected.Count;
                if (tied.Length > remaining)
                {
                    SelectTiedDiscards(tied, remaining, selected);
                    break;
                }

                selected.UnionWith(tied);
                if (selected.Count == count)
                    break;
            }

            return [.. hand.Where(selected.Contains)];

            int Priority(EntityId id)
            {
                if (selection == DiscardSelection.Random)
                    return 0;
                var entity = _state.Entities[id];
                var cost = EffectiveCost(entity, _rules.Card(entity), auras);
                return selection == DiscardSelection.HighestCost ? cost : -cost;
            }
        }

        private void SelectTiedDiscards(EntityId[] candidates, int count, HashSet<EntityId> selected)
        {
            var candidatesLeft = candidates.Length;
            var combinations = CountCombinations(candidatesLeft, count);
            var rank = ChooseIndex(combinations);
            foreach (var candidate in candidates)
            {
                // 按组合抽样，避免搜索把同一批弃牌的不同抽取顺序算成不同结果。
                var withCandidate = combinations * count / candidatesLeft;
                if (rank < withCandidate)
                {
                    selected.Add(candidate);
                    if (--count == 0)
                        return;
                    combinations = withCandidate;
                }
                else
                {
                    rank -= withCandidate;
                    combinations -= withCandidate;
                }
                candidatesLeft--;
            }
        }

        private static int CountCombinations(int candidates, int picks)
        {
            var combinations = 1;
            for (var i = 1; i <= picks; i++)
                combinations = combinations * (candidates - picks + i) / i;
            return combinations;
        }

        private void SummonFromDiscard(Stack<Work> work, EffectContext context)
        {
            var player = context.Controller;
            var id = context.Source;
            if (_state.LocationOf(id) != new EntityLocation(player, GameZone.Graveyard) ||
                _rules.BoardSlotCount(_state, player) >= BoardLimit)
                return;

            SummonExistingMinion(player, id);
            PushSummonTriggers(work, player, id);
        }
    }
}
