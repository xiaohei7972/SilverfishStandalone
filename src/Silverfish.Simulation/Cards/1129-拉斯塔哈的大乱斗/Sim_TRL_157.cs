using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 走跳板
// Walk the Plank
// 消灭一个未受伤的随从。
// Destroy an undamaged minion.
internal static class Sim_TRL_157
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_UNDAMAGED_TARGET)],
        []);
    // </legacy-play-requirements>

}
