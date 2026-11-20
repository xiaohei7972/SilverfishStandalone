namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_144
{
    internal const string CardId = "EX1_144";

    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.FriendlyMinion,
        OnPlay = [new ReturnMinionEffect(CostDelta: -2)]
    };
}
