namespace Silverfish.Simulation.Cards;

[CardSim("CS2_013")]
internal static class Sim_CS2_013
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.GainEmpty, 1, OverflowCardId: "CS2_013t")]
    };
}
