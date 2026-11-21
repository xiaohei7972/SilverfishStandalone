using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 折纸仙鹤
// Origami Crane
// 嘲讽。战吼：与另一个随从交换生命值。
// Taunt Battlecry: Swap Health with another minion.
internal static class Sim_TOY_895
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
