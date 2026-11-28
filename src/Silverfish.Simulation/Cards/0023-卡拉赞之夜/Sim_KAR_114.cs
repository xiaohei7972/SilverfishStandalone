using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 巴内斯
// Barnes
// 战吼：随机挑选你牌库里的一个随从，召唤一个1/1的复制。
// Battlecry: Summon a 1/1 copy of a random minion in your deck.
[CardSim("KAR_114")]
internal static class Sim_KAR_114
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new SummonDeckCopyEffect(ConditionPlayer.Friendly)
            {
                StatsOverride = Sim_KAR_114e.Rule.Modifier,
                EnchantmentCardId = Sim_KAR_114e.Rule.CardId,
                Placement = SummonPlacement.RightOfSource,
            },
        ],
    };
}
