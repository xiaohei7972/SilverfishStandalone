using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 治疗术
// Heal
// 英雄技能 恢复#4点生命值。
// Hero Power Restore #4 Health.
internal static class Sim_CS1h_001_H2_AT_132
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
