namespace Silverfish.Simulation.Cards;

[CardSim("EX1_581")]
internal static class Sim_EX1_581
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.EnemyMinion,
        OnPlay = [new ReturnMinionEffect()]
    };
}
