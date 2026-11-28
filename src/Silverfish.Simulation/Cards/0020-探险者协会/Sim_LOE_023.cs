namespace Silverfish.Simulation.Cards;

// 黑市摊贩
// Dark Peddler
// 战吼：&发现一张法力值消耗为（1）的卡牌。
// Battlecry:& Discover a 1-Cost card.
[CardSim("LOE_023")]
internal static class Sim_LOE_023
{
    internal static CardRules Rules { get; } = new()
    {
        ChooseAfterPlay = new DiscoverCardChoice(new CardFilter { MinimumPrintedCost = 1, MaximumPrintedCost = 1 }),
    };
}
