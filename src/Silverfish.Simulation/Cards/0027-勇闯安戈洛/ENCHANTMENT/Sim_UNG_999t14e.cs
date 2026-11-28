using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 火山之力
// Volcanic Might
// +1/+1。
// +1/+1.
internal static class Sim_UNG_999t14e
{
    internal static StatEnchantmentRule Rule { get; } = new("UNG_999t14e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
