using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 神奇四鱼
// Call in the Finishers
// 召唤四个1/1的鱼人。
// Summon four 1/1 Murlocs.
internal static class Sim_CFM_310
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
