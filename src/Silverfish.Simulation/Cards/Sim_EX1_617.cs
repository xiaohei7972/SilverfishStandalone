namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_617
{
    internal const string CardId = "EX1_617";

    internal static CardRules Rules { get; } = new()
    {
        MinimumEnemyMinions = 1, OnPlay = [new DestroyRandomEnemyMinionEffect()]
    };
}
