namespace Silverfish.Simulation.Cards;

// 被感染的贮藏者
// Polluted Hoarder
// 亡语：抽一张牌。
// Deathrattle: Draw a card.
[CardSim("OG_323")]
internal static class Sim_OG_323
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new DrawEffect(1),
        ],
    };
}
