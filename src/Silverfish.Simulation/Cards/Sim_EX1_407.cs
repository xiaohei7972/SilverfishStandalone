namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_407
{
    internal const string CardId = "EX1_407";

    internal static CardRules Rules { get; } = new()
    {
        MinimumTotalMinions = 2, OnPlay = [new DestroyAllButOneMinionEffect()]
    };
}
