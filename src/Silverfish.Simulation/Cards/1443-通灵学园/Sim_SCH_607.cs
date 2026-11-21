using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 大导师野爪
// Shan'do Wildclaw
// 抉择：使你牌库中的所有野兽牌获得+1/+1；或者变形成为一只友方野兽的复制。
// Choose One - Give Beasts in your deck +1/+1; or Transform into a copy of a friendly Beast.
internal static class Sim_SCH_607
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.BEAST), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
