namespace Silverfish.Simulation.Cards;

// 致命射击
// Deadly Shot
// 随机消灭一个敌方随从。
// Destroy a random enemy minion.
[CardSim("EX1_617")]
internal static class Sim_EX1_617
{
    internal static CardRules Rules { get; } = new()
    {
        MinimumEnemyMinions = 1, OnPlay = [new DestroyRandomEnemyMinionEffect()]
    };
}
