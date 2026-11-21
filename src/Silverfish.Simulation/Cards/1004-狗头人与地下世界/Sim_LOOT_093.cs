using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 战斗号角
// Call to Arms
// 招募三个法力值消耗小于或等于（2）点的随从。
// Recruit 3 minions that cost (2) or less.
internal static class Sim_LOOT_093
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
