using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 走私交易
// Shady Deals
// +1/+1。
// +1/+1.
internal static class Sim_AT_032e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_032e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
