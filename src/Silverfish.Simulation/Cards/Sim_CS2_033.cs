namespace Silverfish.Simulation.Cards;

[CardSim("CS2_033")]
internal static class Sim_CS2_033
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.Damage, TriggerRelation.Friendly, [new FreezeEffect(FreezeTarget.TriggerSubject)])
        {
            Focus = TriggerFocus.DamageSource, EntityMatch = TriggerEntityMatch.Self
        }]
    };
}
