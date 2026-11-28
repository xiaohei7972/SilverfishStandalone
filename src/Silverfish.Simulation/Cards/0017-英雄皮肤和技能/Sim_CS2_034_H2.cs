using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 火焰冲击
// Fireblast
// 英雄技能 造成$1点伤害。
// Hero Power Deal $1 damage.
internal static class Sim_CS2_034_H2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
