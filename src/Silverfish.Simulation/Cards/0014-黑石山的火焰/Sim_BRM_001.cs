namespace Silverfish.Simulation.Cards;

// 严正警戒
// Solemn Vigil
// 抽两张牌。在本回合中每有一个随从死亡，本牌的法力值消耗便减少（1）点。
// Draw 2 cards. Costs (1) less for each minion that died this turn.
[CardSim("BRM_001")]
internal static class Sim_BRM_001
{
    internal static CardRules Rules { get; } = new()
    {
        SelfCostReduction = SelfCostReduction.MinionsDiedThisTurn,
        OnPlay =
        [
            new DrawEffect(2)
        ]
    };
}
