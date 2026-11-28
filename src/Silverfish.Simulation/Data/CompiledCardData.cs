using System.Collections.Immutable;
using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation;

/// <summary>随程序发布的卡牌静态属性。调用方提供卡牌的结算规则。</summary>
public static partial class CompiledCardData
{
    private const int RowWidth = 12;
    private static readonly Lazy<ImmutableDictionary<string, int>> MinionCosts = new(ReadCollectibleMinionCosts);
    private static readonly Lazy<ImmutableHashSet<string>> CollectibleIds = new(ReadCollectibleIds);

    public static int Build => SourceBuild;
    internal static ImmutableDictionary<string, int> CollectibleMinionCosts => MinionCosts.Value;
    internal static ImmutableHashSet<string> CollectibleCardIds => CollectibleIds.Value;

    private static ImmutableHashSet<string> ReadCollectibleIds()
    {
        var rows = Rows;
        var masks = RequirementMasks;
        var ids = CardIds.AsSpan();
        var cards = ImmutableHashSet.CreateBuilder<string>(StringComparer.Ordinal);
        for (var index = 0; index < masks.Length; index++)
        {
            if (!PlayReqTags.Has(masks[index], (int)GAME_TAG.COLLECTIBLE))
                continue;
            var row = index * RowWidth;
            var end = row + RowWidth < rows.Length ? rows[row + RowWidth] : ids.Length;
            cards.Add(ids[rows[row]..end].ToString());
        }
        return cards.ToImmutable();
    }

    private static ImmutableDictionary<string, int> ReadCollectibleMinionCosts()
    {
        var rows = Rows;
        var ids = CardIds.AsSpan();
        var minions = ImmutableDictionary.CreateBuilder<string, int>(StringComparer.Ordinal);
        foreach (var index in CollectibleMinionRows)
        {
            var row = index * RowWidth;
            var end = row + RowWidth < rows.Length ? rows[row + RowWidth] : ids.Length;
            minions.Add(ids[rows[row]..end].ToString(), rows[row + 2]);
        }
        return minions.ToImmutable();
    }

    public static SimulationCard Bind(string cardId, CardRules rules)
    {
        if (string.IsNullOrWhiteSpace(cardId))
            throw new ArgumentException("必须提供卡牌 ID。", nameof(cardId));
        if (rules is null)
            throw new ArgumentNullException(nameof(rules), "必须提供卡牌的结算规则。");

        var rows = Rows;
        var ids = CardIds.AsSpan();
        var count = rows.Length / RowWidth;
        var low = 0;
        var high = count - 1;

        // 按卡牌 ID 的序号顺序查找，单次只读取对应条目。
        while (low <= high)
        {
            var middle = low + (high - low) / 2;
            var row = middle * RowWidth;
            var start = rows[row];
            var end = middle + 1 < count ? rows[row + RowWidth] : ids.Length;
            var order = cardId.AsSpan().CompareTo(ids[start..end], StringComparison.Ordinal);
            if (order == 0)
            {
                var races = (MinionType)((ulong)unchecked((uint)rows[row + 8]) |
                    ((ulong)unchecked((uint)rows[row + 9]) << 32));
                var school = (SpellSchool)rows[row + 10];
                var alternateCost = (TAG_CARD_ALTERNATE_COST)rows[row + 11];
                var definition = new CardDefinition(cardId, (EntityKind)rows[row + 1],
                    PrintedCost: rows[row + 2], PrintedAttack: rows[row + 3],
                    PrintedHealth: rows[row + 4], PrintedDurability: rows[row + 5])
                {
                    MinionTypes = races == MinionType.None ? null : races,
                    SpellSchool = school == SpellSchool.None ? null : school,
                    AlternateCost = alternateCost == TAG_CARD_ALTERNATE_COST.MANA ? null : alternateCost,
                    ClassMask = unchecked((uint)CardClassMasks[middle])
                };
                return new SimulationCard(definition, rules)
                {
                    Keywords = (CardKeywords)rows[row + 6],
                    RequirementTagMask = RequirementMasks[middle], Overload = rows[row + 7]
                };
            }
            if (order < 0)
                high = middle - 1;
            else
                low = middle + 1;
        }
        throw new InvalidDataException($"构建 {SourceBuild} 的内置资料中找不到卡牌「{cardId}」。可以加载对应版本的外部目录，或从完整 CardDefs.xml 重新生成 Generated.g.cs。");
    }
}
