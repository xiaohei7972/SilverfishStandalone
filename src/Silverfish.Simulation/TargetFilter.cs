using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Silverfish.Simulation;

/// <summary>目标必须同时满足全部属性比较和指定的冻结状态。</summary>
public sealed record TargetFilter
{
    public ImmutableArray<TargetComparison> Comparisons { get; init; } = [];
    public bool? Frozen { get; init; }
}

public sealed record TargetComparison(
    [property: JsonRequired] CharacterMetric Metric,
    [property: JsonRequired] NumberComparison Comparison,
    [property: JsonRequired] int Value);
