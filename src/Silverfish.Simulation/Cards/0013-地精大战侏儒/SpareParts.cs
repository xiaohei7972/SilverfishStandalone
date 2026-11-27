using System.Collections.Immutable;

namespace Silverfish.Simulation.Cards;

internal static class SpareParts
{
    internal static ImmutableArray<string> All { get; } =
    [
        "PART_001", "PART_002", "PART_003", "PART_004", "PART_005", "PART_006", "PART_007"
    ];
}
