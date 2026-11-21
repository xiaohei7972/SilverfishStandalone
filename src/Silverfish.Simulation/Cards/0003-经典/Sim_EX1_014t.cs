using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 香蕉
// Bananas
// 使一个随从获得+1/+1。
// Give a minion +1/+1.
internal static class Sim_EX1_014t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
