using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 圣光闪现
// Flash of Light
// 恢复#4点生命值。抽一张牌。
// Restore #4 Health. Draw a card.
internal static class Sim_TRL_307
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
