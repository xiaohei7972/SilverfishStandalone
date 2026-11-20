namespace Silverfish.Simulation.Cards;

[CardSim("UNG_205")]
internal static class Sim_UNG_205
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.EnemyCharacter,
        OnPlay = [new FreezeEffect(FreezeTarget.Selected)]
    };
}
