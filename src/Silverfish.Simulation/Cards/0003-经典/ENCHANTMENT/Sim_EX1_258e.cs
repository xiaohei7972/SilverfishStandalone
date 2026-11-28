using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 过载
// Overloading
// 属性值提高。
// Increased stats.
internal static class Sim_EX1_258e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_258e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
