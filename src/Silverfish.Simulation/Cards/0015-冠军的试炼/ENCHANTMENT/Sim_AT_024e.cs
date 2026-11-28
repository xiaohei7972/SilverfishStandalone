using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 黑暗融合
// Dark Fusion
// +3/+3。
// +3/+3.
internal static class Sim_AT_024e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_024e", new(StatModifierMode.Add, Attack: 3, MaximumHealth: 3));
}
