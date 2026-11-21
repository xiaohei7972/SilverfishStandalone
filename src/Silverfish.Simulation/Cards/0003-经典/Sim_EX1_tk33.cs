using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 地狱火！
// INFERNO!
// 召唤一个6/6的地狱火。
// Summon a 6/6 Infernal.
internal static class Sim_EX1_tk33
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
