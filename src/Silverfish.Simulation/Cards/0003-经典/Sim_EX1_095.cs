namespace Silverfish.Simulation.Cards;

// 加基森拍卖师
// Gadgetzan Auctioneer
// 每当你施放一个法术，抽一张牌。
// Whenever you cast a spell, draw a card.
[CardSim("EX1_095")]
internal static class Sim_EX1_095
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.BeforeSpellResolved, TriggerRelation.Friendly, [new DrawEffect(1)])]
    };
}
