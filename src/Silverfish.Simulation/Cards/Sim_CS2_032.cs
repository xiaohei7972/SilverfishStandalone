namespace Silverfish.Simulation.Cards;

[CardSim("CS2_032")]
internal static class Sim_CS2_032
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.EnemyMinions, 5)]
    };
}
