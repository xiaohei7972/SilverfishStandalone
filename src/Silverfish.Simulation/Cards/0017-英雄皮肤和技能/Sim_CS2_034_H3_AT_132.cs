using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 二级火焰冲击
// Fireblast Rank 2
// 英雄技能 造成$2点伤害。
// Hero Power Deal $2 damage.
internal static class Sim_CS2_034_H3_AT_132
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
