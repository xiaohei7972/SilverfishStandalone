namespace Silverfish.Simulation.Cards;

// 深渊探险
// Journey Below
// 发现一张亡语牌。
// Discover a Deathrattle card.
[CardSim("OG_072")]
internal static class Sim_OG_072
{
    internal static CardRules Rules { get; } = new()
    {
        ChooseAfterPlay = new DiscoverCardChoice(new CardFilter { HasDeathrattle = true }),
    };
}
