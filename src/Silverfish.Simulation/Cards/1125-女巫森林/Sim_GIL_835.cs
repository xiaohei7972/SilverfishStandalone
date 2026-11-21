using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 南瓜宝宝
// Squashling
// 回响，战吼：恢复#2点生命值。
// Echo Battlecry: Restore #2 Health.
internal static class Sim_GIL_835
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
