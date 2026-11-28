namespace Silverfish.Simulation.Cards;

// 宝石甲虫
// Jeweled Scarab
// 战吼：发现一张 法力值消耗为（3）的卡牌。
// Battlecry: Discover a 3-Cost card.
[CardSim("LOE_029")]
internal static class Sim_LOE_029
{
    internal static CardRules Rules { get; } = new()
    {
        ChooseAfterPlay = new DiscoverCardChoice(new CardFilter { MinimumPrintedCost = 3, MaximumPrintedCost = 3 }),
    };
}
