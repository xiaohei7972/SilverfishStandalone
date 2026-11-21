using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 防晒霜
// Sunscreen
// 使一个随从获得+1/+2。
// Give a minion +1/+2.
internal static class Sim_VAC_917t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
