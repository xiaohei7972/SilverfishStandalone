namespace Silverfish.Simulation.Cards.Enchantments;

// 智慧祝福
// Blessing of Wisdom
// 当本随从攻击时，对其施放祝福的玩家抽一张牌。
// When this minion attacks, the player who blessed it draws a card.
internal static class Sim_EX1_363e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_363e", null)
        {
            OnAttack = [new DrawEffect(1)]
        };
}
