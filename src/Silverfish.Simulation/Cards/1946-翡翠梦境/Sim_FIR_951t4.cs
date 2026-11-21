using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 熔岩巨浪
// Lava Wave
// 消耗30份残骸以获得+30/+30。
// Spend 30 Corpses to gain +30/+30.
internal static class Sim_FIR_951t4
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_CORPSES, 30)],
        []);
    // </legacy-play-requirements>

}
