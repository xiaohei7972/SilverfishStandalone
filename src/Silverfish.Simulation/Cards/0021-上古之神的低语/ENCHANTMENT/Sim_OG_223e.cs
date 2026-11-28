using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 圣光之赐
// Optimism
// +1/+2。
// +1/+2.
internal static class Sim_OG_223e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_223e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 2));
}
