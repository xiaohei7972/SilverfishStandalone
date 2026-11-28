namespace Silverfish.Simulation.Cards;

// 海巨人
// Sea Giant
// 战场上每有一个其他随从，本牌的法力值消耗便减少（1）点。
// Costs (1) less for each other minion on the battlefield.
[CardSim("EX1_586")]
internal static class Sim_EX1_586
{
    internal static CardRules Rules { get; } = new()
    {
        SelfCostReduction = SelfCostReduction.OtherMinionsOnBattlefield
    };
}
