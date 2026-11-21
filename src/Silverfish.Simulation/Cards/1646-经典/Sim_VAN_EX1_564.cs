using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 无面操纵者
// Faceless Manipulator
// 战吼：选择一个随从，成为它的复制。
// Battlecry: Choose a minion and become a copy of it.
internal static class Sim_VAN_EX1_564
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
