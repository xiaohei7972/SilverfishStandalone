using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 琥珀看守者
// Amber Watcher
// 战吼： 恢复#8点生命值。
// Battlecry: Restore #8_Health.
internal static class Sim_DRG_226
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
