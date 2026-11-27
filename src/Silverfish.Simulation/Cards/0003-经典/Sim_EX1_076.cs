namespace Silverfish.Simulation.Cards;

// 小个子召唤师
// Pint-Sized Summoner
// 你每个回合使用的第一张随从牌的法力值消耗减少（1）点。
// The first minion you play each turn costs (1) less.
[CardSim("EX1_076")]
internal static class Sim_EX1_076
{
    internal static CardRules Rules { get; } = new()
    {
        CostAuras = [new(CostAuraRecipient.Friendly, CostAuraTarget.Minion, -1) { FirstMinionEachTurn = true }]
    };
}
