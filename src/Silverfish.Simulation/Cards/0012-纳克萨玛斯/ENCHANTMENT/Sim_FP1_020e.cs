using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 复仇之力
// Vengeance
// +3/+2。
// +3/+2.
internal static class Sim_FP1_020e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("FP1_020e", new(StatModifierMode.Add, Attack: 3, MaximumHealth: 2));
}
