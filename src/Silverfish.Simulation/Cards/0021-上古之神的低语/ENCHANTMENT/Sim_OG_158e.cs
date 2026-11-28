using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 新兵手册
// Secrets of the Cult
// +1/+1。
// +1/+1.
internal static class Sim_OG_158e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_158e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
