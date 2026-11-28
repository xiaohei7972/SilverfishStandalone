using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 艾拉隆
// Ara'lon
// 战吼：召唤每种休眠灵种各一个。
// Battlecry: Summon one of each Dormant Wildseed.
internal static class Sim_REV_363
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 2)],
        []);
    // </legacy-play-requirements>

}
