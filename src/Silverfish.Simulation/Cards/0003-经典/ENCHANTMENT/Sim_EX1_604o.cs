using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 狂怒
// Berserk
// 攻击力提高。
// Increased Attack.
internal static class Sim_EX1_604o
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_604o", new(StatModifierMode.Add, Attack: 1));
}
