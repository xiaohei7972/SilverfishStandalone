namespace Silverfish.Simulation.Cards;

// 纳特·帕格
// Nat Pagle
// 在你的回合开始时，你有50%的几率额外抽一张牌。
// At the start of your turn, you have a 50% chance to draw an extra card.
[CardSim("EX1_557")]
internal static class Sim_EX1_557
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.TurnStarted, TriggerRelation.Friendly, [new RandomChanceEffect(1, 2, [new DrawEffect(1)])])]
    };
}
