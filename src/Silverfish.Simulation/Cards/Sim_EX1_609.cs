using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

[CardSim("EX1_609")]
internal static class Sim_EX1_609
{
    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.AfterMinionPlayed, TriggerRelation.Opponent, [new DamageEffect(DamageTarget.TriggerSubject, 6)])
            {
                EntityKind = EntityKind.Minion, RequireLivingEntity = true
            }
        ]
    };
}
