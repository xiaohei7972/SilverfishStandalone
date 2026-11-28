using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 禁忌古树
// Forbidden Ancient
// 战吼：消耗你所有的法力值，每消耗一点法力值，便获得+1/+1。
// Battlecry: Spend all your Mana. Gain +1/+1 for each Mana spent.
[CardSim("OG_051")]
internal static class Sim_OG_051
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new SpendRemainingManaEffect(ManaSpendReward.BuffSource, EnchantmentCardId: Sim_OG_051e.CardId),
        ],
    };
}
