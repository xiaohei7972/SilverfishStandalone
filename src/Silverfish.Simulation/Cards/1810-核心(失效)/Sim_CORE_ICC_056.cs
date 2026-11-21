using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 低温静滞
// Cryostasis
// 使一个随从获得+3/+3，并使其冻结。
// Give a minion +3/+3 and Freeze it.
internal static class Sim_CORE_ICC_056
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
