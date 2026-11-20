namespace Silverfish.Simulation.Cards;

internal static class Sim_CS2_028
{
    internal const string CardId = "CS2_028";

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.EnemyMinions, 2), new FreezeEffect(FreezeTarget.EnemyMinions)]
    };
}
