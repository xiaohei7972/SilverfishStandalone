using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 国王护卫者
// King's Defender
// +1耐久度。
// +1 Durability.
internal static class Sim_AT_065e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_065e", new(StatModifierMode.Add, MaximumHealth: 1));
}
