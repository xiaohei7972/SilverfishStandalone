using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 自然之力
// Force of Nature
// 召唤三个2/2的树人。
// Summon three 2/2 Treants.
internal static class Sim_EX1_571
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
