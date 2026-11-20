namespace Silverfish.Simulation.Cards;

internal static class Sim_CS2_013t
{
    internal const string CardId = "CS2_013t";

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DrawEffect(1)]
    };
}
