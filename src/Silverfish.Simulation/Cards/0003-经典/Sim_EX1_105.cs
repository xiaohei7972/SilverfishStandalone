namespace Silverfish.Simulation.Cards;

// 山岭巨人
// Mountain Giant
// 你每有一张其他手牌，本牌的法力值消耗便减少（1）点。
// Costs (1) less for each other card in your hand.
[CardSim("EX1_105")]
internal static class Sim_EX1_105
{
    internal static CardRules Rules { get; } = new()
    {
        SelfCostReduction = SelfCostReduction.OtherFriendlyHandCards
    };
}
