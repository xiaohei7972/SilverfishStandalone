namespace Silverfish.Simulation.Cards;

// 发条巨人
// Clockwork Giant
// 你的对手每有一张手牌，本牌的法力值消耗便减少（1）点。
// Costs (1) less for each card in your opponent's hand.
[CardSim("GVG_121")]
internal static class Sim_GVG_121
{
    internal static CardRules Rules { get; } = new()
    {
        SelfCostReduction = SelfCostReduction.OpponentHandCards
    };
}
