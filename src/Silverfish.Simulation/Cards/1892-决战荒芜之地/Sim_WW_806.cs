using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 潜踪群蛇
// Sneaky Snakes
// 召唤两条1/1并具有潜行的蛇。
// Summon two 1/1 Snakes with Stealth.
internal static class Sim_WW_806
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
