namespace Silverfish.Simulation.Cards;

// 皇血草毒素
// Kingsblood Toxin
// 抽一张牌。
// Draw a card.
[CardSim("OG_080b")]
internal static class Sim_OG_080b
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DrawEffect(1),
        ],
    };
}
