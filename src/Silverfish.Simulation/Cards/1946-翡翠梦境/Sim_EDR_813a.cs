using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 疫秽蚁群
// Contaminated Colony
// 召唤两只1/1的蚂蚁。
// Summon two 1/1 Ants.
internal static class Sim_EDR_813a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
