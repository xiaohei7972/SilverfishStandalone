using System.Collections.Immutable;
using System.Reflection;

namespace Silverfish.Simulation;

/// <summary>将独立编写的卡牌规则与对应构建的静态属性绑定。</summary>
public static class StarterCardCatalog
{
    private const BindingFlags SimMembers = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private static readonly Lazy<ImmutableArray<(string CardId, CardRules Rules)>> RegisteredRules = new(DiscoverRules);

    private static readonly Lazy<ImmutableArray<SimulationCard>> BuiltInCards = new(() =>
        [.. RegisteredRules.Value.Select(entry => CompiledCardData.Bind(entry.CardId, entry.Rules))]);

    public static ImmutableArray<SimulationCard> Cards => BuiltInCards.Value;

    public static ImmutableArray<SimulationCard> FromCardData(CardDataCatalog catalog) =>
        [.. RegisteredRules.Value.Select(entry => SimulationCard.FromMetadata(catalog.GetCard(entry.CardId), entry.Rules))];

    private static ImmutableArray<(string CardId, CardRules Rules)> DiscoverRules()
    {
        var providers = typeof(StarterCardCatalog).Assembly.GetTypes()
            .OrderBy(type => type.Name, StringComparer.Ordinal);
        var entries = ImmutableArray.CreateBuilder<(string CardId, CardRules Rules)>();
        var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (var provider in providers)
        {
            if (provider.GetCustomAttribute<CardSimAttribute>(inherit: false) is not { } registration)
                continue;

            var cardId = registration.CardId;
            if (string.IsNullOrWhiteSpace(cardId) || provider.Name != $"Sim_{cardId}")
                throw new InvalidOperationException($"单卡 Sim「{provider.FullName}」的 [CardSim] 卡牌 ID 须与类名匹配。");
            if (!ids.Add(cardId))
                throw new InvalidOperationException($"卡牌「{cardId}」有多个标记为 [CardSim] 的规则类。");

            var property = provider.GetProperty("Rules", SimMembers);
            if (property?.PropertyType != typeof(CardRules) || property.GetMethod is null || property.GetIndexParameters().Length != 0)
                throw new InvalidOperationException($"单卡 Sim「{provider.FullName}」须声明可读取的静态 CardRules Rules 属性。");

            try
            {
                var rules = (CardRules?)property.GetValue(null)
                    ?? throw new InvalidOperationException($"卡牌「{cardId}」的 Sim 返回了空规则。");
                entries.Add((cardId, rules));
            }
            catch (TargetInvocationException error)
            {
                throw new InvalidOperationException($"卡牌「{cardId}」的 Sim 初始化失败。", error.InnerException ?? error);
            }
        }
        return entries.ToImmutable();
    }
}
