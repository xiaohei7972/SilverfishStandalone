using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 先行打击
// Precursory Strike
// 造成$3点伤害。如果你的手牌中有法力值消耗大于或等于（5）点的随从牌，抽一张随从牌。
// Deal $3 damage. If you're holding a minion that costs (5) or more, draw a minion.
internal static class Sim_TIME_750
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
