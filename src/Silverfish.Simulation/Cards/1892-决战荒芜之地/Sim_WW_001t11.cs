using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 魔拳岩
// Ogrefist Boulder
// 将一个随从的属性值变为6/7。
// Set a minion's stats to 6/7.
internal static class Sim_WW_001t11
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)]);
    // </legacy-play-requirements>

}
