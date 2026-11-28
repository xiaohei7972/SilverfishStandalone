using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 香蕉
// Bananas
// 拥有+1/+1。
// Has +1/+1.
internal static class Sim_EX1_014te
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_014te", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
