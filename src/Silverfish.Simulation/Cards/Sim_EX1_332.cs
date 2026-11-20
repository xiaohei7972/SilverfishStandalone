namespace Silverfish.Simulation.Cards;

[CardSim("EX1_332")]
internal static class Sim_EX1_332
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.Minion,
        OnPlay = [new SilenceEffect()]
    };
}
