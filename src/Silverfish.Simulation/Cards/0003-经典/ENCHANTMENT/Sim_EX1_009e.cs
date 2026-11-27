using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 激怒
// Enraged
// +5攻击力。
// +5 Attack.
internal static class Sim_EX1_009e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_009e", new(StatModifierMode.Add, Attack: 5));
}
