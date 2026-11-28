namespace Silverfish.Simulation.Cards;

// 攻城车
// Demolisher
// 在你的回合开始时，随机对一个敌人造成2点伤害。
// At the start of your turn, deal 2 damage to a random enemy.
[CardSim("EX1_102")]
internal static class Sim_EX1_102
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.TurnStarted, TriggerRelation.Friendly, [new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 2)])]
    };
}
