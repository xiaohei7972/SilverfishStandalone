using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 圣光的祝福
// Light's Blessing
// 攻击力提高。
// Increased Attack.
internal static class Sim_AT_011e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_011e", new(StatModifierMode.Add, Attack: 2));
}
