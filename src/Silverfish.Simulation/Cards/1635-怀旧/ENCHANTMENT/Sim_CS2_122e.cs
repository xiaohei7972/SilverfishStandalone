using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 强化
// Enhanced
// 团队领袖使本随从获得+1攻击力。
// Raid Leader is granting this minion +1 Attack.
internal static class Sim_CS2_122e
{
    internal const string CardId = "CS2_122e";

    internal static StatEnchantmentRule Rule { get; } = new(CardId,
        new(StatModifierMode.Add, Attack: 1));
}
