namespace Silverfish.Simulation.Cards;

// 熔核巨人
// Molten Giant
// 你的英雄每缺失一点生命值，本牌的法力值消耗便减少（1）点。
// Costs (1) less for each Health your hero is missing.
[CardSim("EX1_620")]
internal static class Sim_EX1_620
{
    internal static CardRules Rules { get; } = new()
    {
        SelfCostReduction = SelfCostReduction.FriendlyHeroMissingHealth
    };
}
