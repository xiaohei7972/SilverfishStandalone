using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 伊莉斯·逐星
// Elise Starseeker
// 战吼：将“黄金猿藏宝图”洗入你的牌库。
// Battlecry: Shuffle the 'Map to the Golden Monkey' into your deck.
[CardSim("LOE_079")]
internal static class Sim_LOE_079
{
    internal static PlayReqSet PlayReqs { get; } = new([], []);

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new CreateCardEffect("LOE_019t", Count: 1, Placement: CardPlacement.ShuffleIntoDeck),
        ],
    };
}
