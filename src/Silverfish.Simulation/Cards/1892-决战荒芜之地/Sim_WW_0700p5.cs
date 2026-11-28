using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 自然枪弹
// Nature Bullet
// 造成$2点伤害。发现一张法术牌。每回合切换。
// Deal $2 damage. Discover a spell. Swaps each turn.
internal static class Sim_WW_0700p5
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
