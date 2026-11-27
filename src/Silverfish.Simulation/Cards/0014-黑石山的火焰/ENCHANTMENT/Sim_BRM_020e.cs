using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 龙人之力
// Draconic Power
// 属性值提高。
// Increased stats.
internal static class Sim_BRM_020e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("BRM_020e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
