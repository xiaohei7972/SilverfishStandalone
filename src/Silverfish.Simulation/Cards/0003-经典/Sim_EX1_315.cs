namespace Silverfish.Simulation.Cards;

// 召唤传送门
// Summoning Portal
// 你的随从牌的法力值消耗减少（2）点，但不能少于（1）点。
// Your minions cost (2) less, but not less than (1).
[CardSim("EX1_315")]
internal static class Sim_EX1_315
{
    internal static CardRules Rules { get; } = new()
    {
        CostAuras = [new(CostAuraRecipient.Friendly, CostAuraTarget.Minion, -2) { MinimumCost = 1 }]
    };
}
