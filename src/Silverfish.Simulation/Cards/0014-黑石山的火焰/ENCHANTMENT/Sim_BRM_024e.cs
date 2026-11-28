using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 巨型龙爪
// Large Talons
// +3/+3。
// +3/+3.
internal static class Sim_BRM_024e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("BRM_024e", new(StatModifierMode.Add, Attack: 3, MaximumHealth: 3));
}
