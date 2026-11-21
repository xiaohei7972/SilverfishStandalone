using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 白银之手
// The Silver Hand
// 英雄技能 召唤两个{0}的白银之手新兵。
// Hero Power Summon two {0} Recruits.
internal static class Sim_CS2_101_H1_AT_132
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
