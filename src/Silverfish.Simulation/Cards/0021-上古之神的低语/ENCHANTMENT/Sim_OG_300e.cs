using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 美味佳肴
// Tasty!
// 属性值提高。
// Increased stats.
internal static class Sim_OG_300e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_300e", new(StatModifierMode.Add, Attack: 2, MaximumHealth: 2));
}
