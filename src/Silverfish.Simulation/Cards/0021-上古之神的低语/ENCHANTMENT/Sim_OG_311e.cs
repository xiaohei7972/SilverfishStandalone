using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 希望信标
// Beacon of Hope
// +2/+2。
// +2/+2.
internal static class Sim_OG_311e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_311e", new(StatModifierMode.Add, Attack: 2, MaximumHealth: 2));
}
