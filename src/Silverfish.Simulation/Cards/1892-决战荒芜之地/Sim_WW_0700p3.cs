using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 火焰枪弹
// Fire Bullet
// 造成$2点伤害，再对所有敌方随从造成$1点伤害。每回合 切换。
// Deal $2 damage, then deal $1 damage to all enemy minions. Swaps each turn.
internal static class Sim_WW_0700p3
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
