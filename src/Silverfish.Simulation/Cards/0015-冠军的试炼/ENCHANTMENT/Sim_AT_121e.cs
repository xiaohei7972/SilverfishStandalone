using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 信心爆棚
// Huge Ego
// 属性值提高。
// Increased stats.
internal static class Sim_AT_121e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_121e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
