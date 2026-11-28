using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 血之契印
// Blood Pact
// 生命值提高。
// Increased Health.
internal static class Sim_CS2_059o
{
    internal static StatEnchantmentRule Rule { get; } =
        new("CS2_059o", new(StatModifierMode.Add, MaximumHealth: 1));
}
