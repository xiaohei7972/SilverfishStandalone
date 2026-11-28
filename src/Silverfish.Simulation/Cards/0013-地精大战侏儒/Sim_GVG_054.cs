namespace Silverfish.Simulation.Cards;

// 食人魔战槌
// Ogre Warmaul
// 50%几率攻击错误的敌人。
// 50% chance to attack the wrong enemy.
[CardSim("GVG_054")]
internal static class Sim_GVG_054
{
    internal static CardRules Rules { get; } = new()
    {
        AttackWrongEnemyChance = true
    };
}
