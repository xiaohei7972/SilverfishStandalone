namespace Silverfish.Simulation.Cards;

// 砂槌萨满祭司
// Dunemaul Shaman
// 风怒，过载：（1） 50%几率攻击错误的敌人。
// Windfury, Overload: (1) 50% chance to attack the wrong enemy.
[CardSim("GVG_066")]
internal static class Sim_GVG_066
{
    internal static CardRules Rules { get; } = new()
    {
        AttackWrongEnemyChance = true
    };
}
