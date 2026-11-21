using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 赎罪教堂
// Cathedral of Atonement
// 使一个随从获得+2/+1。抽一张牌。
// Give a minion +2/+1 and draw a card.
internal static class Sim_REV_791
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)]);
    // </legacy-play-requirements>

}
