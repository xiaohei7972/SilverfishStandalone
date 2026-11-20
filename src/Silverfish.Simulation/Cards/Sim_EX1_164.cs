namespace Silverfish.Simulation.Cards;

[CardSim("EX1_164")]
internal static class Sim_EX1_164
{
    internal static CardRules Rules { get; } = new()
    {
        Choices = [new("gainCrystals", [new ChangeManaEffect(ManaOperation.GainFilled, 2)]), new("draw", [new DrawEffect(3)])]
    };
}
