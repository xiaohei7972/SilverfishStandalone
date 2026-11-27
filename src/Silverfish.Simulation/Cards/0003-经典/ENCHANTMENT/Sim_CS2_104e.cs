using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 狂暴
// Rampage
// +3/+3。
// +3/+3.
internal static class Sim_CS2_104e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("CS2_104e", new(StatModifierMode.Add, Attack: 3, MaximumHealth: 3));
}
