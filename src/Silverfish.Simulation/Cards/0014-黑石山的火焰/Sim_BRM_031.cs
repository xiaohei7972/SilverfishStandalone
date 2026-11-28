namespace Silverfish.Simulation.Cards;

// 克洛玛古斯
// Chromaggus
// 每当你抽一张牌时，将该牌的另一张复制置入你的手牌。
// Whenever you draw a card, put another copy into your hand.
[CardSim("BRM_031")]
internal static class Sim_BRM_031
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.CardDrawn,
                TriggerRelation.Friendly,
                [new CopyTriggerCardToHandEffect(1)])
        ]
    };
}
