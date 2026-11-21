using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 过去的诺莫瑞根
// Past Gnomeregan
// 使一个随从获得+2/+1。推进到现在！
// Give a minion +2/+1. Advance to the present!
internal static class Sim_TIME_044
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)]);
    // </legacy-play-requirements>

}
