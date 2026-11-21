using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 转校生
// Transfer Student
// 已腐蚀
// Corrupted
internal static class Sim_SCH_199t26t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
