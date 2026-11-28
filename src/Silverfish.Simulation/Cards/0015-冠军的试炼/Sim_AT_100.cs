namespace Silverfish.Simulation.Cards;

// 白银之手教官
// Silver Hand Regent
// 激励：召唤一个{0}的白银之手新兵。
// Inspire: Summon a {0} Silver Hand Recruit.
[CardSim("AT_100")]
internal static class Sim_AT_100
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterHeroPowerUsed, TriggerRelation.Friendly, [new SummonEffect("CS2_101t")]),
        ],
    };
}
