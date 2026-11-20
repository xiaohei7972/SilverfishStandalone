namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_581
{
    internal const string CardId = "EX1_581";

    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.EnemyMinion,
        OnPlay = [new ReturnMinionEffect()]
    };
}
