using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 重铸
// Reforged
// +1/+1。
// +1/+1.
internal static class Sim_OG_315e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_315e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
