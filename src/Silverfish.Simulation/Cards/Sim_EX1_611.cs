using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

[CardSim("EX1_611")]
internal static class Sim_EX1_611
{
    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.AttackProposed, TriggerRelation.Opponent,
                [new ReturnMinionEffect(CostDelta: 2, Target: EffectTarget.TriggerSubject)])
            {
                EntityKind = EntityKind.Minion, RequireLivingEntity = true
            }
        ]
    };
}
