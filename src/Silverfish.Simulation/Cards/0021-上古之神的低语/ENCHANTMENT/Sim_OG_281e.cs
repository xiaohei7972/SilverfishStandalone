using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 狂热献祭
// Fanatic Devotion
// 属性值提高。
// Increased Stats.
internal static class Sim_OG_281e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_281e", new(StatModifierMode.Add, Attack: 2, MaximumHealth: 2));
}
