using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 圣光术
// Holy Light
// 恢复#6点生命值。
// Restore #6 Health.
internal static class Sim_VAN_CS2_089
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
