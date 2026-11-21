using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 自埋自收
// Reap What You Sow
// 造成$3点 伤害。发掘一个 宝藏。
// Deal $3 damage. Excavate a treasure.
internal static class Sim_WW_352
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
