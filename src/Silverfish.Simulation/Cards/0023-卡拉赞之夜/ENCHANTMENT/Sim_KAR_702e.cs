using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 简单把戏
// A Simple Trick
// +2/+2。
// +2/+2.
internal static class Sim_KAR_702e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("KAR_702e", new(StatModifierMode.Add, Attack: 2, MaximumHealth: 2));
}
