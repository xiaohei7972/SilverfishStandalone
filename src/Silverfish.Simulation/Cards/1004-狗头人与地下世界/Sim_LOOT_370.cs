using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 寻求组队
// Gather Your Party
// 招募一个随从。
// Recruit a minion.
internal static class Sim_LOOT_370
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
