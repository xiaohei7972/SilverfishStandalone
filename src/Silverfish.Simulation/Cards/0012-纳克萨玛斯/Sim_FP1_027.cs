namespace Silverfish.Simulation.Cards;

// 岩肤石像鬼
// Stoneskin Gargoyle
// 在你的回合开始时，为本随从恢复所有生命值。
// At the start of your turn, restore this minion to full Health.
[CardSim("FP1_027")]
internal static class Sim_FP1_027
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.TurnStarted,
                TriggerRelation.Friendly,
                [new HealEffect(HealTarget.Source, 999)])
        ]
    };
}
