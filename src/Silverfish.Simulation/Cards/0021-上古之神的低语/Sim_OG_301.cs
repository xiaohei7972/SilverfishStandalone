namespace Silverfish.Simulation.Cards;

// 上古之神护卫
// Ancient Shieldbearer
// 战吼： 如果你的克苏恩至少有10点攻击力，则获得10点护甲值。
// Battlecry: If your C'Thun has at least 10 Attack, gain 10 Armor.
[CardSim("OG_301")]
internal static class Sim_OG_301
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(new PermanentCardAttackCondition("OG_280", 10), [new GainArmorEffect(10)]),
        ],
    };
}
