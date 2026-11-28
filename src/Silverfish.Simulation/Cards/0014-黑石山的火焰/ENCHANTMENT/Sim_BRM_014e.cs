using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 狂暴怒意
// Power Rager
// +3/+3
// +3/+3
internal static class Sim_BRM_014e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("BRM_014e", new(StatModifierMode.Add, Attack: 3, MaximumHealth: 3));
}
