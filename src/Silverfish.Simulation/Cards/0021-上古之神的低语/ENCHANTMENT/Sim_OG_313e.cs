using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 腐化
// Addled
// +1/+1。
// +1/+1.
internal static class Sim_OG_313e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_313e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
