using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 辐光雕文
// Glowing Glyph
// 使一个随从及其相邻随从获得+1/+2。
// Give a minion and its neighbors +1/+2.
internal static class Sim_WW_001t8
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
