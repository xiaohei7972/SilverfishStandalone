using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 转校生
// Transfer Student
// 战吼：使一个友方随从获得+1/+2。
// Battlecry: Give a friendly minion +1/+2.
internal static class Sim_SCH_199t3
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
