using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 敲响警钟
// Sound the Bells!
// 回响 使一个随从获得+1/+2。
// Echo Give a minion +1/+2.
internal static class Sim_GIL_145
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
