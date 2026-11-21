using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 圣光术
// Holy Light
// 为你的英雄恢复#8点生命值。
// Restore #8 Health to your hero.
internal static class Sim_CORE_CS2_089
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
