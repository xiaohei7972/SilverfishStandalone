namespace Silverfish.Simulation.Cards;

[CardSim("CS2_026")]
internal static class Sim_CS2_026
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new FreezeEffect(FreezeTarget.EnemyMinions)]
    };
}
