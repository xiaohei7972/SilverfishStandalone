namespace Silverfish.Simulation.Cards;

[CardSim("CS2_013t")]
internal static class Sim_CS2_013t
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DrawEffect(1)]
    };
}
