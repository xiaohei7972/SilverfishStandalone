namespace Silverfish.Simulation.Cards;

internal static class Sim_CS2_114
{
    internal const string CardId = "CS2_114";

    internal static CardRules Rules { get; } = new()
    {
        MinimumEnemyMinions = 1,
        OnPlay = [new RandomDamageEffect(RandomCharacterPool.EnemyMinions, 2, Count: 2, Mode: RandomDamageMode.DistinctBatch)]
    };
}
