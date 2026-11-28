namespace Silverfish.Simulation.Cards;

// 艾维娜
// Aviana
// 你的随从牌的法力值消耗为（1）点。
// Your minions cost (1).
[CardSim("AT_045")]
internal static class Sim_AT_045
{
    internal static CardRules Rules { get; } = new()
    {
        CostAuras = [new(CostAuraRecipient.Friendly, CostAuraTarget.Minion, 0) { SetCost = 1 }],
    };
}
