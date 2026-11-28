using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 灵魂羁绊
// Kindred Spirit
// +3生命值。
// +3 Health.
internal static class Sim_AT_040e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_040e", new(StatModifierMode.Add, MaximumHealth: 3));
}
