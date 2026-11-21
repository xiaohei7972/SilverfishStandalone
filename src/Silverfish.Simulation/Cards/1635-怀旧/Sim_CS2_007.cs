using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 治疗之触
// Healing Touch
// 恢复#8点生命值。
// Restore #8 Health.
internal static class Sim_CS2_007
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
