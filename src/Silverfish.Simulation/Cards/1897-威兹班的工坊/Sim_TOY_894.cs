using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 折纸青蛙
// Origami Frog
// 突袭。战吼：与另一个随从交换攻击力。
// Rush Battlecry: Swap Attack with another minion.
internal static class Sim_TOY_894
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
