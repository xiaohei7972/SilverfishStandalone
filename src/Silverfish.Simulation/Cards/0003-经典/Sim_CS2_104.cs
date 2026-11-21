using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 狂暴
// Rampage
// 使一个受伤的随从获得+3/+3。
// Give a damaged minion +3/+3.
internal static class Sim_CS2_104
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_DAMAGED_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
