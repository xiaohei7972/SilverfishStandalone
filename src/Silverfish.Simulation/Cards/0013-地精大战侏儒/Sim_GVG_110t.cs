namespace Silverfish.Simulation.Cards;

// 砰砰机器人
// Boom Bot
// 亡语：随机对一个敌人造成1-4点伤害。
// Deathrattle: Deal 1-4 damage to a random enemy.
[CardSim("GVG_110t")]
internal static class Sim_GVG_110t
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new RandomDamageRangeEffect(RandomCharacterPool.EnemyCharacters, null, 1, 4)]
    };
}
