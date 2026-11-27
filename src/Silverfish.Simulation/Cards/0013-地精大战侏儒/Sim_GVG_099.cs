namespace Silverfish.Simulation.Cards;

// 榴弹投手
// Bomb Lobber
// 战吼：随机对一个敌方随从造成4点伤害。
// Battlecry: Deal 4 damage to a random enemy minion.
[CardSim("GVG_099")]
internal static class Sim_GVG_099
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new RandomDamageEffect(RandomCharacterPool.EnemyMinions, 4, 1)]
    };
}
