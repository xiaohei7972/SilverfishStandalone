namespace Silverfish.Simulation.Cards;

// 克苏恩
// C'Thun
// 战吼： 造成等同于本随从攻击力的伤害，随机分配到所有敌人身上。
// Battlecry: Deal damage equal to this minion's Attack randomly split among all enemies.
[CardSim("OG_280")]
internal static class Sim_OG_280
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DamageSplitByAttackEffect(),
        ],
    };
}
