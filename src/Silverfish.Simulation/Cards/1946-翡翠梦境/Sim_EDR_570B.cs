using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 动荡能量
// Unstable Power
// 使一个受伤的随从获得+2/+2。
// Give a damaged minion +2/+2.
internal static class Sim_EDR_570B
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_DAMAGED_TARGET)],
        []);
    // </legacy-play-requirements>

}
