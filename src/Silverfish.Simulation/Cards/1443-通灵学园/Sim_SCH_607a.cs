using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 拟态变形
// Transfiguration
// 变形成为一只友方野兽的复制。
// Transform into a copy of a friendly Beast.
internal static class Sim_SCH_607a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.BEAST), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
