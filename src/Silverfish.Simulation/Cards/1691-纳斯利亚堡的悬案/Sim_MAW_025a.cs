using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 有罪！
// Guilty!
// 沉默一个随从。
// Silence a minion.
internal static class Sim_MAW_025a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
