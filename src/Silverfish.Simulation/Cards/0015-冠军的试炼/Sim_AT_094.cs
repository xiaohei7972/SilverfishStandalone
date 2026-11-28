namespace Silverfish.Simulation.Cards;

// 火焰杂耍者
// Flame Juggler
// 战吼：随机对一个敌人造成1点伤害。
// Battlecry: Deal 1 damage to a random enemy.
[CardSim("AT_094")]
internal static class Sim_AT_094
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 1, 1),
        ],
    };
}
