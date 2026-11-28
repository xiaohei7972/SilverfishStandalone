using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 集结号令
// Rally
// +1/+1。
// +1/+1.
internal static class Sim_OG_222e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_222e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
