using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 吃掉小鬼！
// Eat! The! Imp!
// 消灭一个友方随从以抽三张牌。
// Destroy a friendly minion to draw 3 cards.
internal static class Sim_VAC_939
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
