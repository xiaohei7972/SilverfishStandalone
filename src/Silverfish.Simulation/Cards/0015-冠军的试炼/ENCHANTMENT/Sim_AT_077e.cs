using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 额外突刺
// Extra Poke
// +1耐久度。
// +1 Durability.
internal static class Sim_AT_077e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_077e", new(StatModifierMode.Add, MaximumHealth: 1));
}
