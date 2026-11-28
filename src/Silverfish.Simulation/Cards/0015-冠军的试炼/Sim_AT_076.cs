using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 鱼人骑士
// Murloc Knight
// 激励：随机召唤一个鱼人。
// Inspire: Summon a random Murloc.
[CardSim("AT_076")]
internal static class Sim_AT_076
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterHeroPowerUsed, TriggerRelation.Friendly, [new SummonMatchingMinionEffect(new CardFilter { Kind = EntityKind.Minion, MinionType = MinionType.Murloc })]),
        ],
    };
}
