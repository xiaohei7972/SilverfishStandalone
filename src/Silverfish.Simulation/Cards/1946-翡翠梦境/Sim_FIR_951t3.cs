using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 炽热噬咬
// Fiery Chomp
// 消耗20份残骸以获得+20/+20。
// Spend 20 Corpses to gain +20/+20.
internal static class Sim_FIR_951t3
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_CORPSES, 20)],
        []);
    // </legacy-play-requirements>

}
