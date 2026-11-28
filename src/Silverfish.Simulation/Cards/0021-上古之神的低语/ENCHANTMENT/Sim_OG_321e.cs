using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 信仰之力
// Power of Faith
// +1/+1。
// +1/+1.
internal static class Sim_OG_321e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_321e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
