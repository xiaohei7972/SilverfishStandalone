using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 法力吸取
// Mana Gorged
// 攻击力提高。
// Increased attack.
internal static class Sim_NEW1_012o
{
    internal static StatEnchantmentRule Rule { get; } =
        new("NEW1_012o", new(StatModifierMode.Add, Attack: 1));
}
