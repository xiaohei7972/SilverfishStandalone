namespace Silverfish.Simulation.Cards;

[CardSim("CS2_076")]
internal static class Sim_CS2_076
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.EnemyMinion,
        OnPlay = [new DestroyMinionEffect()]
    };
}
