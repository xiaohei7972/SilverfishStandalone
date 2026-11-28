namespace Silverfish.Simulation.Cards;

// 科多兽骑手
// Kodorider
// 激励：召唤一个3/5的作战科多兽。
// Inspire: Summon a 3/5 War Kodo.
[CardSim("AT_099")]
internal static class Sim_AT_099
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterHeroPowerUsed, TriggerRelation.Friendly, [new SummonEffect("AT_099t")]),
        ],
    };
}
