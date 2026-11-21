using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 葛拉卡蟹杀手
// Golakka Glutton
// 战吼：消灭一只野兽，并获得+1/+1。
// Battlecry: Destroy a Beast and gain +1/+1.
internal static class Sim_DED_523
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.PIRATE), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
