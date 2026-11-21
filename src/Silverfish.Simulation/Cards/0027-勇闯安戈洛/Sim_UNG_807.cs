using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 葛拉卡爬行蟹
// Golakka Crawler
// 战吼：消灭一个海盗，并获得+1/+1。
// Battlecry: Destroy a Pirate and gain +1/+1.
internal static class Sim_UNG_807
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.PIRATE), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
