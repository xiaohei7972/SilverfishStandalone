using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 节日庆典
// Ceremony
// +2/+2。
// +2/+2.
internal static class Sim_AT_117e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_117e", new(StatModifierMode.Add, Attack: 2, MaximumHealth: 2));
}
