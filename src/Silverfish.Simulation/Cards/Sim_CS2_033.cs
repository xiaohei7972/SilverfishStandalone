namespace Silverfish.Simulation.Cards;

internal static class Sim_CS2_033
{
    internal const string CardId = "CS2_033";

    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.Damage, TriggerRelation.Friendly, [new FreezeEffect(FreezeTarget.TriggerSubject)])
        {
            Focus = TriggerFocus.DamageSource, EntityMatch = TriggerEntityMatch.Self
        }]
    };
}
