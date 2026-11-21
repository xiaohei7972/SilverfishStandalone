using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 神圣化身
// Apotheosis
// 使一个随从获得+2/+3和吸血。
// Give a minion +2/+3 and Lifesteal.
internal static class Sim_BT_257
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
