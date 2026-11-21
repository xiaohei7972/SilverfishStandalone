using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 猛击
// Slam
// 对一个随从造成$2点伤害，如果 它依然存活，则抽一张牌。
// Deal $2 damage to a minion. If it survives, draw a card.
internal static class Sim_EX1_391
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
