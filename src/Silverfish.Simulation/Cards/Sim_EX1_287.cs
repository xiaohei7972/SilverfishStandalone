namespace Silverfish.Simulation.Cards;

[CardSim("EX1_287")]
internal static class Sim_EX1_287
{
    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.SpellCounterWindow, TriggerRelation.Opponent, [new CounterSpellEffect()])
        ]
    };
}
