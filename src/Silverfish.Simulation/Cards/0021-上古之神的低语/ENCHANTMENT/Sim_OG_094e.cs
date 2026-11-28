using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 触须
// Tentacles
// +2/+6
// +2/+6
internal static class Sim_OG_094e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_094e", new(StatModifierMode.Add, Attack: 2, MaximumHealth: 6));
}
