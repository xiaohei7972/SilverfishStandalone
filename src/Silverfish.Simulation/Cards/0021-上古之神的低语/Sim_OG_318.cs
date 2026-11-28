namespace Silverfish.Simulation.Cards;

// 艾尔文灾星霍格
// Hogger, Doom of Elwynn
// 每当本随从受到伤害，召唤一个2/2并具有嘲讽的豺狼人。
// Whenever this minion takes damage, summon a 2/2 Gnoll with Taunt.
[CardSim("OG_318")]
internal static class Sim_OG_318
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.Damage, TriggerRelation.Friendly, [new SummonEffect("OG_318t")]) { EntityMatch = TriggerEntityMatch.Self },
        ],
    };
}
