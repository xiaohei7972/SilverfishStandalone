using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰爆
// Shatter
// 消灭一个被冻结的随从。
// Destroy a Frozen minion.
internal static class Sim_OG_081
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FROZEN_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
