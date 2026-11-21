using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 新生幼苗
// New Growth
// 召唤两个2/2的树人。
// Summon two 2/2 Treants.
internal static class Sim_BOT_422b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
