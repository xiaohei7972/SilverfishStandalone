namespace Silverfish.Simulation.Cards;

// 加拉克苏斯之拳
// Fist of Jaraxxus
// 当你使用或弃掉这张牌时，随机对一个敌人造成$4点伤害。
// When you play or discard this, deal $4 damage to a random enemy.
[CardSim("AT_022")]
internal static class Sim_AT_022
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 4, 1),
        ],
        WhenDiscarded =
        [
            new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 4, 1),
        ],
    };
}
