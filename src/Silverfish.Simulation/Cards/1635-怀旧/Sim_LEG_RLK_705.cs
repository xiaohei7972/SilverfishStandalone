using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 墓地夜战
// Graveyard Shift
// 召唤两个1/1并具有复生的僵尸。
// Summon two 1/1 Zombies with Reborn.
internal static class Sim_LEG_RLK_705
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
