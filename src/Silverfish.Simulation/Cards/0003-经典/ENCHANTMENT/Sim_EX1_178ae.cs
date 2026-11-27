using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 扎根
// Rooted
// +5生命值并具有嘲讽。
// +5 Health and Taunt.
internal static class Sim_EX1_178ae
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_178ae", new(StatModifierMode.Add, Attack: 5));
}
