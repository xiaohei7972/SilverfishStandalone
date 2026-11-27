namespace Silverfish.Simulation.Cards;

// 狂奔科多兽
// Stampeding Kodo
// 战吼：随机消灭一个攻击力小于或等于2的敌方随从。
// Battlecry: Destroy a random enemy minion with 2 or less Attack.
[CardSim("NEW1_041")]
internal static class Sim_NEW1_041
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DestroyRandomEnemyMinionEffect(MaximumAttack: 2)]
    };
}
