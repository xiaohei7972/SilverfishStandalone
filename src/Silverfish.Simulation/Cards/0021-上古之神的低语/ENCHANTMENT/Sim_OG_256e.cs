using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 黏呼呼的
// Slimed
// +1/+1。
// +1/+1.
internal static class Sim_OG_256e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_256e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
