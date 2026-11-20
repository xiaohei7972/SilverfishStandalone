namespace Silverfish.Simulation.Cards;

[CardSim("EX1_007")]
internal static class Sim_EX1_007
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.Damage, TriggerRelation.Friendly, [new DrawEffect(1)])
        {
            EntityMatch = TriggerEntityMatch.Self
        }]
    };
}
