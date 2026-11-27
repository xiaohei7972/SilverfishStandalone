using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 呀！
// Yarrr!
// +1/+1。
// +1/+1.
internal static class Sim_NEW1_027e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("NEW1_027e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
