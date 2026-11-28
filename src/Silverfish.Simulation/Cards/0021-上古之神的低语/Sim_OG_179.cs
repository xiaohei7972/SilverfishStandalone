namespace Silverfish.Simulation.Cards;

// 炽炎蝙蝠
// Fiery Bat
// 亡语：随机对一个敌人造成1点伤害。
// Deathrattle: Deal 1 damage to a random enemy.
[CardSim("OG_179")]
internal static class Sim_OG_179
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 1, 1),
        ],
    };
}
