using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 召唤宠物
// Call Pet
// 抽一张牌。如果该牌是野兽牌，则其法力值消耗 减少（4）点。
// Draw a card. If it's a Beast, it costs (4) less.
[CardSim("GVG_017")]
internal static class Sim_GVG_017
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DrawAndDiscountIfMatchesEffect(new CardFilter { MinionType = MinionType.Beast }, -4)]
    };
}
