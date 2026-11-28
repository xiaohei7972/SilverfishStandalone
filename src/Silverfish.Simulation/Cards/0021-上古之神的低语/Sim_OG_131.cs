namespace Silverfish.Simulation.Cards;

// 维克洛尔大帝
// Twin Emperor Vek'lor
// 嘲讽，战吼：如果你的克苏恩至少有10点攻击力，则召唤另一名双子皇帝。
// Taunt Battlecry: If your C'Thun has at least 10 Attack, summon another Emperor.
[CardSim("OG_131")]
internal static class Sim_OG_131
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(new PermanentCardAttackCondition("OG_280", 10), [new SummonEffect("OG_319")]),
        ],
    };
}
