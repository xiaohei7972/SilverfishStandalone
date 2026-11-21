using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 时间回溯装置
// Time Rewinder
// 将一个友方随从移回你的手牌。
// Return a friendly minion to your hand.
internal static class Sim_PART_002
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
