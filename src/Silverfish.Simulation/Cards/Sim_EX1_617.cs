namespace Silverfish.Simulation.Cards;

[CardSim("EX1_617")]
internal static class Sim_EX1_617
{
    internal static CardRules Rules { get; } = new()
    {
        MinimumEnemyMinions = 1, OnPlay = [new DestroyRandomEnemyMinionEffect()]
    };
}
