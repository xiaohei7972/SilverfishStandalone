using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 渐近的恐惧
// Encroaching Fear
// 随机召唤两个休眠的魔种。
// Summon two random Dormant Dreadseeds.
internal static class Sim_EDR_820a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
