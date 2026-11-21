using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 时空裂痕
// Time Rip
// 消灭一个随从。祈求迦拉克隆。
// Destroy a minion. Invoke Galakrond.
internal static class Sim_DRG_246
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
