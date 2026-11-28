using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 艾露恩的赐福
// Elune's Grace
// 生命值提高。
// Increased Health.
internal static class Sim_EX1_004e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_004e", new(StatModifierMode.Add, MaximumHealth: 1));
}
