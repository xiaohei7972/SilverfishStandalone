using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 植树造林
// Landscaping
// 召唤两个2/2的树人。
// Summon two 2/2 Treants.
internal static class Sim_CORE_BOT_420
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
