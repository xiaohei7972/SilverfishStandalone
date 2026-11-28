using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 亚煞极印记
// Mark of Y'Shaarj
// +2/+2。
// +2/+2.
internal static class Sim_OG_048e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_048e", new(StatModifierMode.Add, Attack: 2, MaximumHealth: 2));
}
