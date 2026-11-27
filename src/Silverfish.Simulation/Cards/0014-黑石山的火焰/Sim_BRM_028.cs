using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 索瑞森大帝
// Emperor Thaurissan
// 在你的回合结束时，你所有手牌的法力值消耗减少（1）点。
// At the end of your turn, reduce the Cost of cards in your hand by (1).
[CardSim("BRM_028")]
internal static class Sim_BRM_028
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.TurnEnded,
                TriggerRelation.Friendly,
                [
                    new ChangeHandCostEffect(HandCostTarget.FriendlyHand, -1)
                    {
                        EnchantmentCardId = Sim_BRM_028e.CardId
                    }
                ])
        ]
    };
}
