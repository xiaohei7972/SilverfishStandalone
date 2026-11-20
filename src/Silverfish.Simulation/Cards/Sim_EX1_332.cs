namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_332
{
    internal const string CardId = "EX1_332";

    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.Minion,
        OnPlay = [new SilenceEffect()]
    };
}
