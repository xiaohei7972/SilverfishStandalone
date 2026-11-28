namespace Silverfish.Simulation.Cards;

// 顽石元素
// Rumbling Elemental
// 在你使用一张具有 战吼的随从牌后，随机对一个敌人造成2点伤害。
// After you play a Battlecry minion, deal 2 damage to a random enemy.
[CardSim("LOE_016")]
internal static class Sim_LOE_016
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterMinionPlayed, TriggerRelation.Friendly, [new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 2, 1)]) { CardFilter = new CardFilter { HasBattlecry = true } },
        ],
    };
}
