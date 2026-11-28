namespace Silverfish.Simulation.Cards;

// 冰霜巨人
// Frost Giant
// 在本局对战中，你每使用一次英雄技能，本牌的法力值消耗便减少（1）点。
// Costs (1) less for each time you used your Hero Power this game.
[CardSim("AT_120")]
internal static class Sim_AT_120
{
    internal static CardRules Rules { get; } = new()
    {
        SelfCostReduction = SelfCostReduction.HeroPowerUses,
    };
}
