namespace Silverfish.Simulation.Cards;

// 法力晶簇
// Mana Geode
// 过量治疗：召唤一个2/2的水晶。
// Overheal: Summon a 2/2 Crystal.
[CardSim("CFM_606")]
internal static class Sim_CFM_606
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.Overheal, TriggerRelation.Friendly, [new SummonEffect("CFM_606t")])
            {
                Focus = TriggerFocus.EventTarget,
                EntityMatch = TriggerEntityMatch.Self,
            },
        ],
    };
}
