namespace Silverfish.Simulation.Cards;

// 毒镖陷阱
// Dart Trap
// 奥秘： 在对方使用英雄技能后，随机对一个敌人造成$5点伤害。
// Secret: After an opposing Hero Power is used, deal $5 damage to a random enemy.
[CardSim("LOE_021")]
internal static class Sim_LOE_021
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterHeroPowerUsed, TriggerRelation.Opponent, [new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 5, 1)]),
        ],
        IsSecret = true,
    };
}
