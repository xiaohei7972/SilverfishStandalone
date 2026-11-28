using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 激怒
// Enraged
// +6攻击力
// +6 Attack
internal static class Sim_EX1_414e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_414e", new(StatModifierMode.Add, Attack: 6));
}
