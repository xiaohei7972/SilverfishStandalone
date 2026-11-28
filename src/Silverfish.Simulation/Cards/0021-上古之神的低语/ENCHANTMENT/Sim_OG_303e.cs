using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 巫术献祭
// Sorcerous Devotion
// +1/+1。
// +1/+1.
internal static class Sim_OG_303e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_303e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
