using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 装死
// Play Dead
// 触发一个友方随从的亡语。
// Trigger a friendly minion's Deathrattle.
internal static class Sim_CORE_ICC_052
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_TARGET_WITH_DEATHRATTLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
