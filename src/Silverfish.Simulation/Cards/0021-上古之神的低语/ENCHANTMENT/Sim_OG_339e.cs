using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 狂热献祭
// Vassal's Subservience
// +2/+2。
// +2/+2.
internal static class Sim_OG_339e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_339e", new(StatModifierMode.Add, Attack: 2, MaximumHealth: 2));
}
