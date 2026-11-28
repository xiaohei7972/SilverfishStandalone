namespace Silverfish.Simulation.Cards;

// 癫狂的唤冰者
// Demented Frostcaller
// 在你施放一个法术后，随机冻结 一个敌人。
// After you cast a spell, Freeze a random enemy.
[CardSim("OG_085")]
internal static class Sim_OG_085
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterSpellResolved, TriggerRelation.Friendly, [new FreezeEffect(FreezeTarget.RandomUnfrozenEnemy)]),
        ],
    };
}
