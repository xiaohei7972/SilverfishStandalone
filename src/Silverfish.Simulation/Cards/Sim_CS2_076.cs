namespace Silverfish.Simulation.Cards;

internal static class Sim_CS2_076
{
    internal const string CardId = "CS2_076";

    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.EnemyMinion,
        OnPlay = [new DestroyMinionEffect()]
    };
}
