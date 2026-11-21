using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 潮汐之力
// The Tidal Hand
// 召唤一个1/1的白银之手鱼人。
// Summon a 1/1 Silver Hand Murloc.
internal static class Sim_OG_006b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
