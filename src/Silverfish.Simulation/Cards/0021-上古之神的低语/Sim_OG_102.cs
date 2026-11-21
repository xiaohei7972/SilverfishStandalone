using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 黑暗低语者
// Darkspeaker
// 战吼：与另一个友方随从交换属性值。
// Battlecry: Swap stats with a friendly minion.
internal static class Sim_OG_102
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
