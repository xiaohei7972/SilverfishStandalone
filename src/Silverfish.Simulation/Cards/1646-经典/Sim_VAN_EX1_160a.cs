using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 召唤猎豹
// Summon a Panther
// 召唤一只3/2的猎豹。
// Summon a 3/2 Panther.
internal static class Sim_VAN_EX1_160a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
