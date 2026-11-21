using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 并蒂树苗
// One, Two, Trees!
// 召唤两个1/1的树苗。
// Summon two 1/1 Saplings.
internal static class Sim_AT_037b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
