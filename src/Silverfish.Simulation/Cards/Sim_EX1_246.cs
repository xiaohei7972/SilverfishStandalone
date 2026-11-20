namespace Silverfish.Simulation.Cards;

[CardSim("EX1_246")]
internal static class Sim_EX1_246
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.Minion,
        OnPlay = [new TransformMinionEffect("hexfrog")]
    };
}
