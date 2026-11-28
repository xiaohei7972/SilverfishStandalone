using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 能量之光
// Lantern of Power
// +10/+10。
// +10/+10.
internal static class Sim_LOEA16_3e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("LOEA16_3e", new(StatModifierMode.Add, Attack: 10, MaximumHealth: 10));
}
