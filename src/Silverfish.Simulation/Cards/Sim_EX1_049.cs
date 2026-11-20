namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_049
{
    internal const string CardId = "EX1_049";

    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.FriendlyMinion,
        OnPlay = [new ReturnMinionEffect()]
    };
}
