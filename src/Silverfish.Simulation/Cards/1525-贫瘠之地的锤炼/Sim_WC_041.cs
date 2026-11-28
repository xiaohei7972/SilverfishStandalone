using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰爆冲击
// Shattering Blast
// 消灭所有被冻结的随从。
// Destroy all Frozen minions.
internal static class Sim_WC_041
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_FROZEN_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
