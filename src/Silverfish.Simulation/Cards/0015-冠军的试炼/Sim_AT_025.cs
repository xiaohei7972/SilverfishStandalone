using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 黑暗交易
// Dark Bargain
// 随机消灭两个敌方随从，随机弃两张牌。
// Destroy 2 random enemy minions. Discard 2 random cards.
internal static class Sim_AT_025
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
