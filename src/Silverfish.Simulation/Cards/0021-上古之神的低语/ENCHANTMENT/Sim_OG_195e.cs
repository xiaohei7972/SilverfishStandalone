using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 身强体壮
// Enormous
// +2/+2。
// +2/+2.
internal static class Sim_OG_195e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_195e", new(StatModifierMode.Add, Attack: 2, MaximumHealth: 2));
}
