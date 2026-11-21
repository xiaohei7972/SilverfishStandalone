using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 关门放狗
// Unleash the Hounds
// 战场上每有一个敌方随从，便召唤一个 1/1并具有冲锋的猎犬。
// For each enemy minion, summon a 1/1 Hound with Charge.
internal static class Sim_VAN_EX1_538
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1), new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
