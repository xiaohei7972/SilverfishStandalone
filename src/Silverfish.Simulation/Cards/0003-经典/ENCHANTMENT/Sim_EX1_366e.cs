using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 伸张正义
// Justice Served
// 拥有+1/+1。
// Has +1/+1.
internal static class Sim_EX1_366e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_366e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
