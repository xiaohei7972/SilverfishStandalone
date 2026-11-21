using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 提振士气
// Improve Morale
// 对一个随从造成$1点伤害。如果它依然存活，则将一张跟班牌置入你的手牌。
// Deal $1 damage to a minion. If it survives, add a Lackey to your hand.
internal static class Sim_DAL_769
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
