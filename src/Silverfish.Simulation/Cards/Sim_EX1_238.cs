namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_238
{
    internal const string CardId = "EX1_238";

    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.Character,
        OnPlay = [new DamageEffect(DamageTarget.Selected, 3)]
    };
}
