using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 邪火之怒
// Felrage
// 属性值提高。
// Increased stats.
internal static class Sim_AT_021e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_021e", new(StatModifierMode.Add, Attack: 2, MaximumHealth: 1));
}
