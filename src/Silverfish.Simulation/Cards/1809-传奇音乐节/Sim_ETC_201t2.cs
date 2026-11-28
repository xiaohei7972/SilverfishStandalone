using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 一串香蕉
// Bunch of Bananas
// 使一个随从获得+1/+1。（最后一根香蕉！）
// Give a minion +1/+1. (Last Banana!)
internal static class Sim_ETC_201t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
