using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 兔妖教头
// Virmen Sensei
// 战吼：使一个友方野兽获得+3/+3。
// Battlecry: Give a friendly Beast +3/+3.
internal static class Sim_WON_300
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.BEAST), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
