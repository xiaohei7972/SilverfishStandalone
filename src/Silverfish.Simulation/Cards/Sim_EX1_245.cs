namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_245
{
    internal const string CardId = "EX1_245";

    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.Minion,
        OnPlay = [new SilenceEffect(), new DamageEffect(DamageTarget.Selected, 1)]
    };
}
