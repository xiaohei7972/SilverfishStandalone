using System.Numerics;
using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private bool IsOtherClassCard(GameState state, PlayerId player, SimulationCard candidate)
    {
        var classes = candidate.Definition.ClassMask;
        var heroClasses = Card(state.Entities[state.Players[player].Hero]).Definition.ClassMask;
        return classes != 0 && (classes & (heroClasses | NeutralClass)) == 0;
    }

    private bool IsValidCapturedMinion(GameState state, CapturedMinion? capture) => capture is null ||
        (state.Players.ContainsKey(capture.Controller) && !string.IsNullOrWhiteSpace(capture.CardId) &&
            GetCard(capture.CardId).Definition.Kind == EntityKind.Minion);

    private sealed partial class Resolution
    {
        private void DestroyAndCaptureMinion(EffectContext context, DestroyAndCaptureMinionEffect effect)
        {
            if (context.Selected is not EntityId target || !IsLivingCharacter(target))
                return;

            var minion = _state.Entities[target];
            var capture = new CapturedMinion(minion.CardId, minion.Controller);
            EnchantStats(context, effect.Enchantment, capture);
            DestroyMinion(target);
        }

        private void EnchantMinionsByType(EffectContext context, EnchantMinionsByTypeEffect effect)
        {
            var board = _rules.BoardMinions(_state, context.Controller).Where(id => IsLivingCharacter(id)).ToArray();
            if (board.Length == 0)
                return;

            // 多种族随从只能占一个名额。先求出可覆盖最多种族的受益者组合。
            var eligible = effect.Types.Select(type => board.Select((id, index) =>
                MatchesMinionType(_rules.Card(_state.Entities[id]).Definition, type) ? 1u << index : 0).ToArray()).ToArray();
            var best = new HashSet<uint>();
            var visited = new HashSet<(int Type, uint Selected)>();
            var maximum = 0;
            Assign(0, 0);
            if (maximum == 0)
                return;

            var recipients = best.Order().ElementAt(ChooseIndex(best.Count));
            for (var index = 0; index < board.Length; index++)
                if ((recipients & (1u << index)) != 0)
                    EnchantStats(context with { Selected = board[index] }, effect.Enchantment);

            void Assign(int type, uint selected)
            {
                if (!visited.Add((type, selected)))
                    return;
                if (type == eligible.Length)
                {
                    var count = BitOperations.PopCount(selected);
                    if (count > maximum)
                    {
                        maximum = count;
                        best.Clear();
                    }
                    if (count == maximum)
                        best.Add(selected);
                    return;
                }

                foreach (var mask in eligible[type])
                    if (mask != 0 && (selected & mask) == 0)
                        Assign(type + 1, selected | mask);
                Assign(type + 1, selected);
            }
        }
    }
}
