using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 集群撕咬
// Devouring Swarm
// 选择一个敌方随从，使你的所有随从攻击该随从，并将死亡的友方随从移回你的手牌。
// Choose an enemy minion. Your minions attack it, then return any that die to your hand.
internal static class Sim_SW_460
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
