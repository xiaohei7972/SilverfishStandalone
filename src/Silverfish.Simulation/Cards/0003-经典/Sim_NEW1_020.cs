namespace Silverfish.Simulation.Cards;

// 狂野炎术师
// Wild Pyromancer
// 在你施放一个法术后，&对所有随从造成1点伤害。
// After you cast a spell,& deal 1 damage to ALL minions.
[CardSim("NEW1_020")]
internal static class Sim_NEW1_020
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.AfterSpellResolved, TriggerRelation.Friendly, [new DamageEffect(DamageTarget.AllMinions, 1)])]
    };
}
