using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 我是鱼人
// I Am Murloc
// 召唤三个、四个或者五个1/1的 鱼人。
// Summon three, four, or five 1/1 Murlocs.
internal static class Sim_PRO_001a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
