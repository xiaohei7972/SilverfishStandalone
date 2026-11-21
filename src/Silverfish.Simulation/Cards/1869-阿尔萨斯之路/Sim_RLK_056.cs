using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 邪恶狂热
// Unholy Frenzy
// 选择一个敌方随从，使你的所有随从攻击该随从。再次召唤死亡的友方随从。
// Choose an enemy minion. Your minions attack it. Resummon any that die.
internal static class Sim_RLK_056
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
