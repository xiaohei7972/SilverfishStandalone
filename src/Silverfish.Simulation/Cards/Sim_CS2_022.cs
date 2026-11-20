namespace Silverfish.Simulation.Cards;

internal static class Sim_CS2_022
{
    internal const string CardId = "CS2_022";

    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.Minion,
        OnPlay = [new TransformMinionEffect("CS2_tk1")]
    };
}
