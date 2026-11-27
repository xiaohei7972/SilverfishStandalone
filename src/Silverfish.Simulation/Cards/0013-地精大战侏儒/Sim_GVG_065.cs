namespace Silverfish.Simulation.Cards;

// 食人魔步兵
// Ogre Brute
// 50%几率攻击错误的敌人。
// 50% chance to attack the wrong enemy.
[CardSim("GVG_065")]
internal static class Sim_GVG_065
{
    internal static CardRules Rules { get; } = new()
    {
        AttackWrongEnemyChance = true
    };
}
