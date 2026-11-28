using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 毒蛇神殿传送门
// Serpentshrine Portal
// 造成$3点伤害。随机召唤一个法力值消耗为（3）的随从。过载：（1）
// Deal $3 damage. Summon a random 3-Cost minion. Overload: (1)
internal static class Sim_BT_100
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
