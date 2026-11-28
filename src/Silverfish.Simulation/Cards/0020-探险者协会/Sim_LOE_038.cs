namespace Silverfish.Simulation.Cards;

// 纳迦海巫
// Naga Sea Witch
// 你的卡牌法力值消耗为（5）点。
// Your cards cost (5).
[CardSim("LOE_038")]
internal static class Sim_LOE_038
{
    internal static CardRules Rules { get; } = new()
    {
        CostAuras = [new(CostAuraRecipient.Friendly, CostAuraTarget.Hand, 0) { SetCost = 5 }],
    };
}
