namespace Silverfish.Simulation.Cards;

internal static class Sim_CS2_032
{
    internal const string CardId = "CS2_032";

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.EnemyMinions, 5)]
    };
}
