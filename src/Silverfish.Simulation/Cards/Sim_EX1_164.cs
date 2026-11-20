namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_164
{
    internal const string CardId = "EX1_164";

    internal static CardRules Rules { get; } = new()
    {
        Choices = [new("gainCrystals", [new ChangeManaEffect(ManaOperation.GainFilled, 2)]), new("draw", [new DrawEffect(3)])]
    };
}
