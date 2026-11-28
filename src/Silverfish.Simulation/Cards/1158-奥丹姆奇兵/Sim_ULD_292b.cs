using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 分流出击
// Divide and Conquer
// 召唤一个本随从的复制。
// Summon a copy of this_minion.
internal static class Sim_ULD_292b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 2)],
        []);
    // </legacy-play-requirements>

}
