using System.Collections.Immutable;
using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private const int EvolveCostLimit = 10;
    private readonly Lazy<ImmutableDictionary<int, ImmutableArray<string>>> _randomMinionCosts;
    public ImmutableArray<string>? RandomMinionPool { get; }

    private ImmutableDictionary<int, ImmutableArray<string>> BuildRandomMinionCosts()
    {
        var costs = RandomMinionPool is { } pool ? pool.Distinct(StringComparer.Ordinal).Select(id =>
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("随机随从池中的卡牌 ID 不能为空。", nameof(RandomMinionPool));
            var card = GetCard(id);
            if (card.Definition.Kind != EntityKind.Minion || card.Definition.PrintedCost < 0)
                throw new ArgumentException($"随机随从池中的卡牌「{id}」须为具有有效费用的随从。", nameof(RandomMinionPool));
            if (PlayReqTags.Has(card.RequirementTagMask, (int)GAME_TAG.TITAN))
                throw new ArgumentException($"泰坦「{id}」不能加入随机随从池。", nameof(RandomMinionPool));
            return new KeyValuePair<string, int>(id, card.Definition.PrintedCost);
        }) : DefaultMinionCosts();
        return costs.GroupBy(card => card.Value)
            .ToImmutableDictionary(group => group.Key, group => group.Select(card => card.Key).Order(StringComparer.Ordinal).ToImmutableArray());
    }

    private static IEnumerable<KeyValuePair<string, int>> DefaultMinionCosts()
    {
        var cards = CompiledCardData.CollectibleMinionCosts;
        return cards.Where(card => !card.Key.StartsWith("CORE_", StringComparison.Ordinal) || !cards.ContainsKey(card.Key[5..]));
    }

    private ImmutableArray<string> EvolveCandidates(CardDefinition original, int increase)
    {
        var pool = _randomMinionCosts.Value;
        if (original.PrintedCost > EvolveCostLimit)
            return pool.TryGetValue(original.PrintedCost, out var sameCost) ? sameCost : [original.CardId];

        var desiredCost = (int)Math.Min(EvolveCostLimit, (long)original.PrintedCost + increase);
        if (pool.TryGetValue(desiredCost, out var candidates))
            return candidates;

        // 缺档时从最接近的费用选牌。距离相同时先取较低的费用档。
        var closest = pool.Keys.Where(cost => cost <= EvolveCostLimit)
            .OrderBy(cost => Math.Abs(cost - desiredCost)).ThenBy(cost => cost).Select(cost => (int?)cost).FirstOrDefault();
        return closest is int available ? pool[available] : [];
    }

    private sealed partial class Resolution
    {
        private void TransformMinionByCost(Stack<Work> work, EffectContext context, int increase)
        {
            var id = context.Selected!.Value;
            if (!IsLivingCharacter(id))
                return;
            var original = _rules.Card(_state.Entities[id]).Definition;
            if (original.Kind != EntityKind.Minion)
                return;
            var candidates = _rules.EvolveCandidates(original, increase);
            if (!candidates.IsEmpty)
                TransformMinion(work, context, candidates[ChooseIndex(candidates.Length)]);
        }
    }
}
