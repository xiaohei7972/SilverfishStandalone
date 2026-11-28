namespace Silverfish.Simulation.Cards;

// 轮回
// Effigy
// 奥秘：当一个友方随从死亡时，随机召唤一个法力值消耗相同的随从。
// Secret: When a friendly minion dies, summon a random minion with the same Cost.
[CardSim("AT_002")]
internal static class Sim_AT_002
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.MinionDied, TriggerRelation.Friendly, [new SummonMatchingCostEffect()]) { RequiresBoardSpace = true },
        ],
        IsSecret = true,
    };
}
