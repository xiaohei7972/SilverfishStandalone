namespace Silverfish.Simulation.Cards;

// 玛克扎尔的小鬼
// Malchezaar's Imp
// 每当你弃掉一张牌时，抽一张牌。
// Whenever you discard a card, draw a card.
[CardSim("KAR_089")]
internal static class Sim_KAR_089
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.CardDiscarded, TriggerRelation.Friendly, [new DrawEffect(1)]),
        ],
    };
}
