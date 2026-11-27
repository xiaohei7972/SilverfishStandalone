using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 兽群领袖
// Leader of the Pack
// +1/+1。
// +1/+1.
internal static class Sim_EX1_160be
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_160be", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
