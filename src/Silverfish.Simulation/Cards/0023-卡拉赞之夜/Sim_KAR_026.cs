using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 保卫国王
// Protect the King!
// 战场上每有一个敌方随从，便召唤一个1/1并具有嘲讽的 禁卫。
// For each enemy minion, summon a 1/1 Pawn with Taunt.
internal static class Sim_KAR_026
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1), new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
