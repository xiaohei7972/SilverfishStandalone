using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 奋勇争先
// Caller Devotion
// +1/+1。
// +1/+1.
internal static class Sim_OG_290e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_290e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
