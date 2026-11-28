namespace Silverfish.Simulation.Cards;

// 夜色镇图书管理员
// Darkshire Librarian
// 战吼： 随机弃一张牌。 亡语： 抽一张牌。
// Battlecry: Discard a random card. Deathrattle: Draw a card.
[CardSim("OG_109")]
internal static class Sim_OG_109
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DiscardEffect(),
        ],
        Deathrattle =
        [
            new DrawEffect(1),
        ],
    };
}
