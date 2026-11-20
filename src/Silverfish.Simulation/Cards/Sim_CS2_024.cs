namespace Silverfish.Simulation.Cards;

[CardSim("CS2_024")]
internal static class Sim_CS2_024
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.Character,
        OnPlay = [new DamageEffect(DamageTarget.Selected, 3), new FreezeEffect(FreezeTarget.Selected)]
    };
}
