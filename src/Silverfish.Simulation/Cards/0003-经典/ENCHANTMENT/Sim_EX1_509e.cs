using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 吧啦咯咯啦！
// Blarghghl
// 攻击力提高。
// Increased Attack.
internal static class Sim_EX1_509e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_509e", new(StatModifierMode.Add, Attack: 1));
}
