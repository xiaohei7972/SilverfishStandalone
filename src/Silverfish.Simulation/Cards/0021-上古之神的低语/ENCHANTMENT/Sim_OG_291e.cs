using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 浮光暗影
// Flickering Darkness
// 暗影施法者将它变成了1/1。
// Shadowcaster made this 1/1.
internal static class Sim_OG_291e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_291e", new(StatModifierMode.Set, Attack: 1, MaximumHealth: 1));
}
