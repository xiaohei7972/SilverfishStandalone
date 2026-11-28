using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰刺
// Icicle
// 对一个随从造成$2点伤害。如果它已被冻结，抽一张牌。
// Deal $2 damage to a minion. If it's Frozen, draw a card.
internal static class Sim_EX1_179
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
