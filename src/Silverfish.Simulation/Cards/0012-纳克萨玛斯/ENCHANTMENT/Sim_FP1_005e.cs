using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 吞噬
// Consume
// 属性值提高。
// Increased stats.
internal static class Sim_FP1_005e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("FP1_005e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
