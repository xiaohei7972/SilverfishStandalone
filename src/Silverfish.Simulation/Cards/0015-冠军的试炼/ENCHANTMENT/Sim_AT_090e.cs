using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 金刚之力
// Might of the Monkey
// +1/+1。
// +1/+1.
internal static class Sim_AT_090e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_090e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
