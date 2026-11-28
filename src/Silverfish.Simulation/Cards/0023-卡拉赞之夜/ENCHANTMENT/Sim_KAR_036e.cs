using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 吞食
// Eating
// 生命值提高。
// Increased Health.
internal static class Sim_KAR_036e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("KAR_036e", new(StatModifierMode.Add, MaximumHealth: 1));
}
