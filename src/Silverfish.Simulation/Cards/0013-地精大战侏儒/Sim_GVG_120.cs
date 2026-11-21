using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 赫米特·奈辛瓦里
// Hemet Nesingwary
// 战吼： 消灭一个野兽。
// Battlecry: Destroy a Beast.
internal static class Sim_GVG_120
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.BEAST)],
        []);
    // </legacy-play-requirements>

}
