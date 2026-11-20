using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Silverfish.Simulation;

/// <summary>目标必须满足指定的属性、冻结状态和卡牌条件。</summary>
public sealed record TargetFilter
{
    public ImmutableArray<TargetComparison> Comparisons { get; init; } = [];
    public bool? Frozen { get; init; }
    public CardFilter? CardFilter { get; init; }
}

public sealed record TargetComparison(
    [property: JsonRequired] CharacterMetric Metric,
    [property: JsonRequired] NumberComparison Comparison,
    [property: JsonRequired] int Value);
