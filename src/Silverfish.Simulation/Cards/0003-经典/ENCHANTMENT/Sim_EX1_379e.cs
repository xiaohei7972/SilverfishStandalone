using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 忏悔
// Repentance
// 生命值降为1。
// Health reduced to 1.
internal static class Sim_EX1_379e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_379e", new(StatModifierMode.Set, MaximumHealth: 1));
}
