using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 岩质甲壳
// Rocky Carapace
// +3生命值。
// +3 Health.
internal static class Sim_UNG_999t4e
{
    internal static StatEnchantmentRule Rule { get; } = new("UNG_999t4e", new(StatModifierMode.Add, MaximumHealth: 3));
}
