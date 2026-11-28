using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 月圆之赐
// Night's Devotion
// +1/+1。
// +1/+1.
internal static class Sim_OG_292e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_292e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
