using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 强心迷雾
// Empowering Mist
// +1/+1。
// +1/+1.
internal static class Sim_AT_045e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_045e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
