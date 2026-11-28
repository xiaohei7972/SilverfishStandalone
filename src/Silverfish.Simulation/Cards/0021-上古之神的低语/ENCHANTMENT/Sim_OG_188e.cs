using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 琥珀甲壳
// Amber Carapace
// +5生命值。
// +5 Health.
internal static class Sim_OG_188e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_188e", new(StatModifierMode.Add, MaximumHealth: 5));
}
