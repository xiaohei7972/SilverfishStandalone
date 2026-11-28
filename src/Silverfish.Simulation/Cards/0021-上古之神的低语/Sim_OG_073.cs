using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 菊花茶
// Thistle Tea
// 抽一张牌。将两张该牌的复制置入你的手牌。
// Draw a card. Add 2 extra copies of it to your hand.
[CardSim("OG_073")]
internal static class Sim_OG_073
{
    internal static PlayReqSet PlayReqs { get; } = new([], []);

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DrawEffect(1) { ExtraCopies = 2 },
        ],
    };
}
