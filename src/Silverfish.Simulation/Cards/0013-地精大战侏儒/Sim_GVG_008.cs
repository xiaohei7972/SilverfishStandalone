namespace Silverfish.Simulation.Cards;

// 圣光炸弹
// Lightbomb
// 对所有随从造成等同于其攻击力的伤害。
// Deal damage to each minion equal to its Attack.
[CardSim("GVG_008")]
internal static class Sim_GVG_008
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DealAttackAsDamageToAllMinionsEffect()]
    };
}
