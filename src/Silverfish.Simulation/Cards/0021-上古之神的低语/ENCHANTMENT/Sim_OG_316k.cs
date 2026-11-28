using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 暗影力量
// Shadowy
// 1/1。
// 1/1.
internal static class Sim_OG_316k
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_316k", new(StatModifierMode.Set, Attack: 1, MaximumHealth: 1));
}
