using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 黑暗召唤
// Darkness Calls
// 属性值提高。
// Increased stats.
internal static class Sim_FP1_028e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("FP1_028e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
