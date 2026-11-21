using System.Collections.Immutable;
using System.Reflection;

namespace Silverfish.Simulation;

/// <summary>将独立编写的卡牌规则与对应构建的静态属性绑定。</summary>
public static class StarterCardCatalog
{
    private const BindingFlags SimMembers = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private static readonly Lazy<ImmutableArray<(string CardId, CardRules Rules)>> Rules = new(DiscoverRules);

    private static readonly Lazy<ImmutableArray<SimulationCard>> BuiltInCards = new(() =>
        [.. RegisteredRules.Select(entry => CompiledCardData.Bind(entry.CardId, entry.Rules))]);

    internal static ImmutableArray<(string CardId, CardRules Rules)> RegisteredRules => Rules.Value;

    public static ImmutableArray<SimulationCard> Cards => BuiltInCards.Value;

    public static ImmutableArray<SimulationCard> FromCardData(CardDataCatalog catalog) =>
        [.. RegisteredRules.Select(entry => SimulationCard.FromMetadata(catalog.GetCard(entry.CardId), entry.Rules))];

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
                var requirements = PlayReqCatalog.ReadDeclaredRequirements(provider) ?? new([], []);
                ValidateRequirementPlacement(cardId, rules, requirements);
                entries.Add((cardId, rules with { PlayReqs = requirements }));
            }
            catch (TargetInvocationException error)
            {
                throw new InvalidOperationException($"卡牌「{cardId}」的 Sim 初始化失败。", error.InnerException ?? error);
            }
        }
        return entries.ToImmutable();
    }

    private static void ValidateRequirementPlacement(string cardId, CardRules rules, PlayReqSet requirements)
    {
        if (rules.PlayReqs is not null || rules.Target != TargetRequirement.None || rules.TargetFilter is not null ||
            rules.Requirements != PlayRequirements.None || rules.MinimumEnemyMinions != 0 || rules.MinimumTotalMinions != 0)
            throw new InvalidOperationException($"卡牌「{cardId}」的出牌条件必须声明在 Sim 类的静态 PlayReqs.Play 中。");
        if (rules.Activation is { Target: not TargetRequirement.None } or { TargetFilter: not null })
            throw new InvalidOperationException($"卡牌「{cardId}」的激活条件必须声明在 Sim 类的静态 PlayReqs.Ability 中。");
        if (!rules.Choices.IsDefaultOrEmpty && rules.Choices.Any(choice => choice is
            { Target: not TargetRequirement.None } or { TargetFilter: not null } or { Requirements: not PlayRequirements.None }))
            throw new InvalidOperationException($"卡牌「{cardId}」的分支条件必须声明在对应选项的 CardPlayChoice.PlayReqs 中。");
        ValidateRequirementList(cardId, "PlayReqs.Play", requirements.Play);
        ValidateRequirementList(cardId, "PlayReqs.Ability", requirements.Ability);
        foreach (var choice in rules.Choices.IsDefault ? [] : rules.Choices)
        {
            ValidateRequirementList(cardId, $"CardPlayChoice.PlayReqs（{choice.Id}）", choice.PlayReqs);
            if (choice.PlayReqs.Any(requirements.Play.Contains))
                throw new InvalidOperationException($"卡牌「{cardId}」的抉择「{choice.Id}」重复声明了公共出牌条件。");
        }
    }

    private static void ValidateRequirementList(string cardId, string group, ImmutableArray<PlayReq> requirements)
    {
        if (requirements.IsDefault || requirements.Any(requirement => requirement is null) ||
            requirements.Distinct().Count() != requirements.Length)
            throw new InvalidOperationException($"卡牌「{cardId}」的 {group} 必须是已初始化且无重复项的条件列表。");
    }
}
