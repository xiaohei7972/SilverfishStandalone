namespace Silverfish.Simulation.Cards;

[CardSim("EX1_251")]
internal static class Sim_EX1_251
{
    internal static CardRules Rules { get; } = new()
    {
        MinimumEnemyMinions = 1,
        OnPlay = [new RandomDamageEffect(RandomCharacterPool.EnemyMinions, 2, Count: 2, Mode: RandomDamageMode.DistinctBatch)]
    };
}
