using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 绿皮的命令
// Greenskin's Command
// +1/+1。
// +1/+1.
internal static class Sim_NEW1_024o
{
    internal static StatEnchantmentRule Rule { get; } =
        new("NEW1_024o", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
