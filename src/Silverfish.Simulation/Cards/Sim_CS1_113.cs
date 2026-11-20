namespace Silverfish.Simulation.Cards;

internal static class Sim_CS1_113
{
    internal const string CardId = "CS1_113";

    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.EnemyMinion,
        OnPlay = [new TakeControlEffect()]
    };
}
