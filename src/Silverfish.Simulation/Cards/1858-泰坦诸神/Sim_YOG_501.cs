using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 历战无面者
// Battleworn Faceless
// 战吼：变形成为一个受伤的随从的复制。
// Battlecry: Transform into a copy of a damaged minion.
internal static class Sim_YOG_501
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_DAMAGED_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
