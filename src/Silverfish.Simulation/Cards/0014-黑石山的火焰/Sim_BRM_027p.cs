namespace Silverfish.Simulation.Cards;

// 死吧，虫子！
// DIE, INSECT!
// 随机对一个敌人造成8点伤害。
// Deal 8 damage to a random enemy.
[CardSim("BRM_027p")]
internal static class Sim_BRM_027p
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 8, 1)
        ]
    };
}
