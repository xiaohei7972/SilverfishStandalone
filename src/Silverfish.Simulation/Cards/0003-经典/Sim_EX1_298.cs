namespace Silverfish.Simulation.Cards;

// 炎魔之王拉格纳罗斯
// Ragnaros the Firelord
// 无法攻击。在你的回合结束时，随机对一个敌人造成8点伤害。
// Can't attack. At the end of your turn, deal 8 damage to a random enemy.
[CardSim("EX1_298")]
internal static class Sim_EX1_298
{
    internal static CardRules Rules { get; } = new()
    {
        CannotAttack = true,
        Triggers = [new(TriggerTiming.TurnEnded, TriggerRelation.Friendly, [new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 8)])]
    };
}
