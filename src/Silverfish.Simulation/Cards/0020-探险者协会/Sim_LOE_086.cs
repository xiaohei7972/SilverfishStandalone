namespace Silverfish.Simulation.Cards;

// 召唤石
// Summoning Stone
// 每当你施放一个法术，随机召唤一个法力值消耗相同的随从。
// Whenever you cast a spell, summon a random minion of the same Cost.
[CardSim("LOE_086")]
internal static class Sim_LOE_086
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.BeforeSpellResolved, TriggerRelation.Friendly, [new SummonMatchingCostEffect()]),
        ],
    };
}
