using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 挑剔的观众
// Tough Crowd
// 流放：将一个随从移回其拥有者的手牌。
// Outcast: Return a minion to its owner's hand.
internal static class Sim_JAM_020
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
