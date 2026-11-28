using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 嗜血之刃
// Thirsty Blades
// +1/+1。
// +1/+1.
internal static class Sim_OG_070e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_070e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
