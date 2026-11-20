namespace Silverfish.Simulation.Cards;

// 闷棍
// Sap
// 将一个敌方随从移回你对手的 手牌。
// Return an enemy minion to your opponent's hand.
[CardSim("EX1_581")]
internal static class Sim_EX1_581
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.EnemyMinion,
        OnPlay = [new ReturnMinionEffect()]
    };
}
