using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 小鬼集群（等级3）
// Imp Swarm (Rank 3)
// 召唤三个3/2的小鬼。
// Summon three 3/2 Imps.
internal static class Sim_BAR_914t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
