using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 黑暗卫士
// Dark Guardian
// 属性值提高。
// Increased Stats.
internal static class Sim_OG_293f
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_293f", new(StatModifierMode.Add, Attack: 4, MaximumHealth: 4));
}
