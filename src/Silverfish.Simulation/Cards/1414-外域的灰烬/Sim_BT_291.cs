using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 埃匹希斯冲击
// Apexis Blast
// 造成$5点伤害。如果你的牌库中没有随从牌，随机召唤一个法力值消耗为（5）的随从。
// Deal $5 damage. If your deck has no minions, summon a random 5-Cost minion.
internal static class Sim_BT_291
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
