using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 锦鱼人水语者
// Jinyu Waterspeaker
// 战吼：恢复#6点生命值。过载：（1）
// Battlecry: Restore #6 Health. Overload: (1)
internal static class Sim_CFM_061
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
