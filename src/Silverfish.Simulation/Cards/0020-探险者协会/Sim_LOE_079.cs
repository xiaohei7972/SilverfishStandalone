using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 伊莉斯·逐星
// Elise Starseeker
// 战吼：将“黄金猿藏宝图”洗入你的牌库。
// Battlecry: Shuffle the 'Map to the Golden Monkey' into your deck.
internal static class Sim_LOE_079
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
