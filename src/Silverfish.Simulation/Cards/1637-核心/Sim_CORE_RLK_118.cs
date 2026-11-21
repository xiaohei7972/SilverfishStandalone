using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 坟墓守卫
// Tomb Guardians
// 召唤两个2/2并具有嘲讽的僵尸。消耗4份残骸，使其获得复生。
// Summon two 2/2 Zombies with Taunt. Spend 4 Corpses to give them Reborn.
internal static class Sim_CORE_RLK_118
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
