using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 半神的恩赐
// Demigod's Favor
// +2/+2。
// +2/+2.
internal static class Sim_EX1_573ae
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_573ae", new(StatModifierMode.Add, Attack: 2, MaximumHealth: 2));
}
