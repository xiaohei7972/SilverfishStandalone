namespace Silverfish.Simulation.Cards;

// 莫罗斯
// Moroes
// 潜行 在你的回合结束时，召唤一个1/1的 家仆。
// Stealth At the end of your turn, summon a 1/1 Steward.
[CardSim("KAR_044")]
internal static class Sim_KAR_044
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.TurnEnded, TriggerRelation.Friendly, [new SummonEffect("KAR_044a", SummonPlacement.RightOfSource)]),
        ],
    };
}
