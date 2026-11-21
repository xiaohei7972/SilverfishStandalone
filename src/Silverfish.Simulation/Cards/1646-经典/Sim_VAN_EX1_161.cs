using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 自然平衡
// Naturalize
// 消灭一个随从，你的对手抽两张牌。
// Destroy a minion. Your opponent draws 2_cards.
internal static class Sim_VAN_EX1_161
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
