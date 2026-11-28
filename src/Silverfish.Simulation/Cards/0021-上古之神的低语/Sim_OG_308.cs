namespace Silverfish.Simulation.Cards;

// 巨型沙虫
// Giant Sand Worm
// 每当本随从攻击并消灭一个随从，可再次攻击。
// Whenever this attacks and kills a minion, it may attack again.
[CardSim("OG_308")]
internal static class Sim_OG_308
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterAttack, TriggerRelation.Friendly, [new RefreshAttackEffect()]) { EntityMatch = TriggerEntityMatch.Self, KilledMinion = true },
        ],
    };
}
