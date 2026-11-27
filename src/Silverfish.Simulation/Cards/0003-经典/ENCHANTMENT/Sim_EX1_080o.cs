using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 守护奥秘
// Keeping Secrets
// 属性值提高。
// Increased stats.
internal static class Sim_EX1_080o
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_080o", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
