namespace Silverfish.Simulation.Cards;

// 基维斯
// Jeeves
// 在每个玩家的回合结束时，该玩家抽若干牌，直至其手牌数量达到3张。
// At the end of each player's turn, that player draws until they have 3 cards.
[CardSim("GVG_094")]
internal static class Sim_GVG_094
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.TurnEnded,
                TriggerRelation.Any,
                [new DrawToHandSizeEffect(3, CardRecipient.CurrentTurnPlayer)])
        ]
    };
}
