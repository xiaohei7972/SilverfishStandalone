namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_251
{
    internal const string CardId = "EX1_251";

    internal static CardRules Rules { get; } = new()
    {
        MinimumEnemyMinions = 1,
        OnPlay = [new RandomDamageEffect(RandomCharacterPool.EnemyMinions, 2, Count: 2, Mode: RandomDamageMode.DistinctBatch)]
    };
}
