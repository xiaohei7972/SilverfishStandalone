namespace Silverfish.Simulation.Cards;

[CardSim("CS2_028")]
internal static class Sim_CS2_028
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.EnemyMinions, 2), new FreezeEffect(FreezeTarget.EnemyMinions)]
    };
}
