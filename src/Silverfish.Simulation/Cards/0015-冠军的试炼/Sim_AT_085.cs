namespace Silverfish.Simulation.Cards;

// 湖之仙女
// Maiden of the Lake
// 你的英雄技能的法力值消耗为（1）点。
// Your Hero Power costs (1).
[CardSim("AT_085")]
internal static class Sim_AT_085
{
    internal static CardRules Rules { get; } = new()
    {
        CostAuras = [new(CostAuraRecipient.Friendly, CostAuraTarget.HeroPower, 0) { SetCost = 1 }],
    };
}
