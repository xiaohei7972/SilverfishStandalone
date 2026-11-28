namespace Silverfish.Simulation.Cards;

// 巨型蟾蜍
// Huge Toad
// 亡语：随机对一个敌人造成1点伤害。
// Deathrattle: Deal 1 damage to a random enemy.
[CardSim("LOE_046")]
internal static class Sim_LOE_046
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 1, 1),
        ],
    };
}
