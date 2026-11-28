namespace Silverfish.Simulation.Cards;

// 黑暗邪使艾蒂丝
// Eydis Darkbane
// 每当你以本随从为目标施放一个法术时，随机对一个敌人造成3点伤害。
// Whenever you target this minion with a spell, deal 3 damage to a random enemy.
[CardSim("AT_131")]
internal static class Sim_AT_131
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.TargetedBySpell, TriggerRelation.Friendly, [new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 3, 1)]) { EntityMatch = TriggerEntityMatch.Self },
        ],
    };
}
