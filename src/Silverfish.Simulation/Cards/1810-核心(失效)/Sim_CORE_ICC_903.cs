using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 血色狂欢者
// Sanguine Reveler
// 战吼：消灭一个友方随从，并获得+2/+2。
// Battlecry: Destroy a friendly minion and gain_+2/+2.
internal static class Sim_CORE_ICC_903
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
