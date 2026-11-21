using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 初始之火
// First Flame
// 对一个随从造成$2点伤害。将“传承之火”置入你的手牌。
// Deal $2 damage to a minion. Add a Second Flame to your hand.
internal static class Sim_SW_108
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
