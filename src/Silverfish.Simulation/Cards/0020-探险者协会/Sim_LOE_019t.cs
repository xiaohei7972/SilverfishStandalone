namespace Silverfish.Simulation.Cards;

// 黄金猿藏宝图
// Map to the Golden Monkey
// 将“黄金猿”洗入你的牌库。抽一张牌。
// Shuffle the Golden Monkey into your deck. Draw a card.
[CardSim("LOE_019t")]
internal static class Sim_LOE_019t
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new CreateCardEffect("LOE_019t2", Count: 1, Placement: CardPlacement.ShuffleIntoDeck),
            new DrawEffect(1),
        ],
    };
}
