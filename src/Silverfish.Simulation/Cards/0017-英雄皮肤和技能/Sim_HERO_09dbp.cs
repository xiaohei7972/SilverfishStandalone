using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 次级治疗术
// Lesser Heal
// 英雄技能 恢复#2点生命值。
// Hero Power Restore #2 Health.
internal static class Sim_HERO_09dbp
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
