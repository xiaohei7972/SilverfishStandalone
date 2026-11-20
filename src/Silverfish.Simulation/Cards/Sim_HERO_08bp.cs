namespace Silverfish.Simulation.Cards;

[CardSim("HERO_08bp")]
internal static class Sim_HERO_08bp
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.Character,
        OnPlay = [new DamageEffect(DamageTarget.Selected, 1)]
    };
}
