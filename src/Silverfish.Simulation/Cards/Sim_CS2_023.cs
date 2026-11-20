namespace Silverfish.Simulation.Cards;

internal static class Sim_CS2_023
{
    internal const string CardId = "CS2_023";

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DrawEffect(2)]
    };
}
