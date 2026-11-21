namespace Silverfish.Simulation.Cards;

// 飞刀杂耍者
// Knife Juggler
// 在你召唤一个随从后，随机对一个敌人造成1点伤害。
// After you summon a minion, deal 1 damage to a random enemy.
[CardSim("NEW1_019")]
internal static class Sim_NEW1_019
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.AfterMinionSummoned, TriggerRelation.Friendly,
            [new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 1)])]
    };
}
