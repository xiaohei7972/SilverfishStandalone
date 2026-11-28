using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 闪电链（等级3）
// Chain Lightning (Rank 3)
// 对一个随从和随机一个相邻随从造成$4点伤害。
// Deal $4 damage to a minion and a random adjacent one.
internal static class Sim_BAR_044t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
