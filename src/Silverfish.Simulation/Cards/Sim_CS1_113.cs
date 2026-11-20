namespace Silverfish.Simulation.Cards;

[CardSim("CS1_113")]
internal static class Sim_CS1_113
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.EnemyMinion,
        OnPlay = [new TakeControlEffect()]
    };
}
