using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 死灵毒药
// Necrotic Poison
// 消灭一个随从。
// Destroy a minion.
internal static class Sim_VAC_464t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
