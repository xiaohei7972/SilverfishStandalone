using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 新生闪电
// Nascent Bolt
// 对一个随从造成$5点伤害。如果该随从依然存活，抽两张牌。
// Deal $5 damage to a minion. If it survives, draw 2 cards.
internal static class Sim_TIME_216
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
