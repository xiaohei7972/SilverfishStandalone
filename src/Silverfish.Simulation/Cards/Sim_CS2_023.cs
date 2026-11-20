namespace Silverfish.Simulation.Cards;

[CardSim("CS2_023")]
internal static class Sim_CS2_023
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DrawEffect(2)]
    };
}
