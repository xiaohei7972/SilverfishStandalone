using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 受到鼓舞
// Inspired
// 属性值提高。
// Increased stats.
internal static class Sim_AT_119e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_119e", new(StatModifierMode.Add, Attack: 2, MaximumHealth: 2));
}
