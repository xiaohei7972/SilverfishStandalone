namespace Silverfish.Simulation.Cards;

// 穆戈尔的勇士
// Mogor's Champion
// 50%几率攻击错误的敌人。
// 50% chance to attack the wrong enemy.
[CardSim("AT_088")]
internal static class Sim_AT_088
{
    internal static CardRules Rules { get; } = new()
    {
        AttackWrongEnemyChance = true,
    };
}
