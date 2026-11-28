using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 白银之力
// Silver Might
// +2/+2。
// +2/+2.
internal static class Sim_KAR_077e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("KAR_077e", new(StatModifierMode.Add, Attack: 2, MaximumHealth: 2));
}
