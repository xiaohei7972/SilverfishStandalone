using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 激怒
// Enraged
// +2攻击力。
// +2 Attack.
internal static class Sim_OG_150e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_150e", new(StatModifierMode.Add, Attack: 2));
}
