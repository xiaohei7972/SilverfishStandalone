using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 深坑鳄鱼
// Pit Crocolisk
// 战吼：造成5点伤害。
// Battlecry: Deal 5 damage.
internal static class Sim_ULD_190
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
