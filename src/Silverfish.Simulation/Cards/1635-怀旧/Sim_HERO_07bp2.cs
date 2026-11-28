namespace Silverfish.Simulation.Cards;

// 灵魂分流
// Soul Tap
// 英雄技能 抽一张牌。
// Hero Power Draw a card.
[CardSim("HERO_07bp2")]
internal static class Sim_HERO_07bp2
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DrawEffect(1),
        ],
    };
}
