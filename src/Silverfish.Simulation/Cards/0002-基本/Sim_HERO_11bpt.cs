namespace Silverfish.Simulation.Cards;

// 脆弱的食尸鬼
// Frail Ghoul
// 冲锋。在你的回合结束时死亡。
// Charge At the end of your turn, this minion dies.
[CardSim("HERO_11bpt")]
internal static class Sim_HERO_11bpt
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.TurnEnded, TriggerRelation.Friendly, [new DestroyMinionEffect(EffectTarget.Source)]),
        ],
    };
}
