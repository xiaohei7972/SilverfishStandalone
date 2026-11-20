namespace Silverfish.Simulation.Cards;

[CardSim("EX1_144")]
internal static class Sim_EX1_144
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.FriendlyMinion,
        OnPlay = [new ReturnMinionEffect(CostDelta: -2)]
    };
}
