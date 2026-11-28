namespace Silverfish.Simulation.Cards;

// 救赎
// Redemption
// 奥秘：当一个友方随从死亡时，使其回到战场，并具有1点生命值。
// Secret: When a friendly minion dies, return it to life with 1 Health.
[CardSim("EX1_136")]
internal static class Sim_EX1_136
{
    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.MinionDied, TriggerRelation.Friendly, [new ResummonMinionEffect(EffectTarget.TriggerSubject, RemainingHealth: 1)])
            {
                RequiresBoardSpace = true
            }
        ]
    };
}
