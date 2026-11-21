using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 嗜魔增幅
// Empowered
// 魔瘾者的攻击力提高。
// Mana Addict has increased Attack.
internal static class Sim_EX1_055o
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_055o", new(StatModifierMode.Add, Attack: 2), EnchantmentDuration.CurrentTurn);
}
