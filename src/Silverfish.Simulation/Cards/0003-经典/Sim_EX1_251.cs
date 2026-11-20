namespace Silverfish.Simulation.Cards;

// 叉状闪电
// Forked Lightning
// 随机对两个敌方随从造成$2点伤害，过载：（2）
// Deal $2 damage to 2_random enemy minions. Overload: (2)
[CardSim("EX1_251")]
internal static class Sim_EX1_251
{
    internal static CardRules Rules { get; } = new()
    {
        MinimumEnemyMinions = 1,
        OnPlay = [new RandomDamageEffect(RandomCharacterPool.EnemyMinions, 2, Count: 2, Mode: RandomDamageMode.DistinctBatch)]
    };
}
