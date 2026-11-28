using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 上紧发条
// Wound Up
// +1/+1。
// +1/+1.
internal static class Sim_AT_096e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_096e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
