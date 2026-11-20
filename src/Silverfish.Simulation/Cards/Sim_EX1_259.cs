namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_259
{
    internal const string CardId = "EX1_259";

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.EnemyMinions, 3)]
    };
}
