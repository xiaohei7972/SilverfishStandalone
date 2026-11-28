namespace Silverfish.Simulation.Cards;

// 风险投资公司雇佣兵
// Venture Co. Mercenary
// 你的随从牌的法力值消耗增加（3）点。
// Your minions cost (3) more.
[CardSim("CS2_227")]
internal static class Sim_CS2_227
{
    internal static CardRules Rules { get; } = new()
    {
        CostAuras = [new(CostAuraRecipient.Friendly, CostAuraTarget.Minion, 3)]
    };
}
