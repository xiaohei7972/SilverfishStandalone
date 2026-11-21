using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 火焰之地传送门
// Firelands Portal
// 造成$6点伤害。随机召唤一个法力值消耗为（6）的随从。
// Deal $6 damage. Summon a random 6-Cost minion.
internal static class Sim_KAR_076
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
