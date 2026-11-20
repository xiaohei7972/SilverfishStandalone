namespace Silverfish.Simulation.Cards;

// 精神控制
// Mind Control
// 夺取一个敌方随从的控制权。
// Take control of an enemy minion.
[CardSim("CS1_113")]
internal static class Sim_CS1_113
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.EnemyMinion,
        OnPlay = [new TakeControlEffect()]
    };
}
