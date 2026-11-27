namespace Silverfish.Simulation.Cards;

// 食人魔忍者
// Ogre Ninja
// 潜行，50%几率攻击错误的敌人。
// Stealth 50% chance to attack the wrong enemy.
[CardSim("GVG_088")]
internal static class Sim_GVG_088
{
    internal static CardRules Rules { get; } = new()
    {
        AttackWrongEnemyChance = true
    };
}
