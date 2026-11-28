using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 巨化术
// Dinosize
// 将一个随从的属性值变为7/14。
// Set a minion's stats to 7/14.
internal static class Sim_UNG_004
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
