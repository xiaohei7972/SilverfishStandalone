using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 小鬼集群（等级2）
// Imp Swarm (Rank 2)
// 召唤两个3/2的小鬼。（当你有10点法力值时升级。）
// Summon two 3/2 Imps. (Upgrades when you have 10 Mana.)
internal static class Sim_BAR_914t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
