using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 致命射击
// Deadly Shot
// 随机消灭一个敌方随从。
// Destroy a random enemy minion.
internal static class Sim_VAN_EX1_617
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
