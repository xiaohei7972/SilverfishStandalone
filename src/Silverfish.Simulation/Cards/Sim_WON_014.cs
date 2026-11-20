namespace Silverfish.Simulation.Cards;

internal static class Sim_WON_014
{
    internal const string CardId = "WON_014";

    internal static CardRules Rules { get; } = new()
    {
        Choices = [new("gainCrystal", [new ChangeManaEffect(ManaOperation.GainEmpty, 1)]), new("draw", [new DrawEffect(1)])]
    };
}
