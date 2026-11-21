using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 举尾重压
// Tail Smash
// 消耗10份残骸以获得+10/+10。
// Spend 10 Corpses to gain +10/+10.
internal static class Sim_FIR_951t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_CORPSES, 10)],
        []);
    // </legacy-play-requirements>

}
