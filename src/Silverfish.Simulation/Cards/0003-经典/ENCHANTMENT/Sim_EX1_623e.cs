using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 生命灌注
// Infusion
// +3生命值
// +3 Health.
internal static class Sim_EX1_623e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_623e", new(StatModifierMode.Add, MaximumHealth: 3));
}
