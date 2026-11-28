using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 争强好胜
// Competitive Spirit
// +1/+1。
// +1/+1.
internal static class Sim_AT_073e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_073e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
