using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 龙血
// Dragon Blood
// +1/+1
// +1/+1
internal static class Sim_BRM_033e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("BRM_033e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
