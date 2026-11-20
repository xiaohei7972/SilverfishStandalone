namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_241
{
    internal const string CardId = "EX1_241";

    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.Character,
        OnPlay = [new DamageEffect(DamageTarget.Selected, 5)]
    };
}
