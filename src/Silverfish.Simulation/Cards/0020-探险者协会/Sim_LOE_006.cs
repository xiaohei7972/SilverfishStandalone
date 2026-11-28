namespace Silverfish.Simulation.Cards;

// 博物馆馆长
// Museum Curator
// 战吼：发现一张亡语牌，其法力值消耗减少（1）点。
// Battlecry: Discover a Deathrattle card. It costs (1) less.
[CardSim("LOE_006")]
internal static class Sim_LOE_006
{
    internal static CardRules Rules { get; } = new()
    {
        ChooseAfterPlay = new DiscoverCardChoice(new CardFilter { HasDeathrattle = true }) { CostDelta = -1 },
    };
}
