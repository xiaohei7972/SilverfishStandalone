using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 驯兽师
// Houndmaster
// 战吼：使一个友方野兽获得+2/+2和嘲讽。
// Battlecry: Give a friendly Beast +2/+2 and Taunt.
internal static class Sim_VAN_DS1_070
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.BEAST), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
