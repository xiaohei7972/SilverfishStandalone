namespace Silverfish.Simulation.Cards;

[CardSim("BRM_011")]
internal static class Sim_BRM_011
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.Character,
        OnPlay = [new DamageEffect(DamageTarget.Selected, 2), new UnlockOverloadEffect()]
    };
}
