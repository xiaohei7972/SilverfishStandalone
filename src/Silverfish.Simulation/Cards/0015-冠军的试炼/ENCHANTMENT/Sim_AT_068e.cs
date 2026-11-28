using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 加固
// Bolstered
// +2/+2。
// +2/+2.
internal static class Sim_AT_068e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_068e", new(StatModifierMode.Add, Attack: 2, MaximumHealth: 2));
}
