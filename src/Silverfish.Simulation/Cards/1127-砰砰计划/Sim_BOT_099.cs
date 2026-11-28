using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 我找到了
// Eureka!
// 随机召唤你手牌中的一张随从牌的一个复制。
// Summon a copy of_a_random minion from your hand.
internal static class Sim_BOT_099
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
