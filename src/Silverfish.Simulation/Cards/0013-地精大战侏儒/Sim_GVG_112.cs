namespace Silverfish.Simulation.Cards;

// 食人魔勇士穆戈尔
// Mogor the Ogre
// 所有随从有50%几率攻击错误的敌人。
// All minions have a 50% chance to attack the wrong enemy.
[CardSim("GVG_112")]
internal static class Sim_GVG_112
{
    internal static CardRules Rules { get; } = new()
    {
        AllMinionsAttackWrongEnemyChance = true
    };
}
