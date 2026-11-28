using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 原始的能量！
// Raw Power!
// 属性值提高。
// Increased stats.
internal static class Sim_EX1_274e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_274e", new(StatModifierMode.Add, Attack: 2, MaximumHealth: 2));
}
