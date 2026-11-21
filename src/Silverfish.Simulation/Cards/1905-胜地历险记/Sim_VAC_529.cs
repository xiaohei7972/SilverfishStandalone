using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 游学生
// Scrapbooking Student
// 战吼：召唤一个友方地标的复制。
// Battlecry: Summon a copy of a friendly location.
internal static class Sim_VAC_529
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_LOCATION_TARGET)],
        []);
    // </legacy-play-requirements>

}
