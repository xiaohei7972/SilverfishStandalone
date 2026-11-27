using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 范克里夫的复仇
// VanCleef's Vengeance
// 属性值提高。
// Increased stats.
internal static class Sim_EX1_613e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_613e", new(StatModifierMode.Add, Attack: 2, MaximumHealth: 2));
}
