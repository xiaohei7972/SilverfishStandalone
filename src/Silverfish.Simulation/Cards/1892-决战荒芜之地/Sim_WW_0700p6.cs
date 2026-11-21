using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暗影枪弹
// Shadow Bullet
// 造成$2点伤害。随机召唤一个法力值消耗为（3）的随从。每回合切换。
// Deal $2 damage. Summon a random 3-Cost minion. Swaps each turn.
internal static class Sim_WW_0700p6
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
