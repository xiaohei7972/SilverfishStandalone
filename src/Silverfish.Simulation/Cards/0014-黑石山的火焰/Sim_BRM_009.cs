namespace Silverfish.Simulation.Cards;

// 火山邪木
// Volcanic Lumberer
// 嘲讽，在本回合中每有一个随从死亡，本牌的法力值消耗便减少（1）点。
// Taunt. Costs (1) less for each minion that died this turn.
[CardSim("BRM_009")]
internal static class Sim_BRM_009
{
    internal static CardRules Rules { get; } = new()
    {
        SelfCostReduction = SelfCostReduction.MinionsDiedThisTurn
    };
}
