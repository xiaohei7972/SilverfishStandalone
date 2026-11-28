using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 永歌传送门
// Eversong Portal
// 召唤$1只4/4并具有突袭的山猫（受法术伤害加成影响）。
// Summon $1 4/4 |4(Lynx, Lynxes) with Rush (improved by Spell Damage).
internal static class Sim_RLK_819
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
