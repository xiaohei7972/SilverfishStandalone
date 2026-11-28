using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 胜利！
// Victory!
// +1/+1。
// +1/+1.
internal static class Sim_AT_133e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_133e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
