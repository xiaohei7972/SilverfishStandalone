namespace Silverfish.Simulation.Cards;

// 火山幼龙
// Volcanic Drake
// 在本回合中每有一个随从死亡，本牌的法力值消耗便减少（1）点。
// Costs (1) less for each minion that died this turn.
[CardSim("BRM_025")]
internal static class Sim_BRM_025
{
    internal static CardRules Rules { get; } = new()
    {
        SelfCostReduction = SelfCostReduction.MinionsDiedThisTurn
    };
}
