using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 愤怒
// Wrath
// 抉择： 对一个随从造成$3点伤害；或者造成$1点伤害并抽一张牌。
// Choose One - Deal $3 damage to a minion; or $1 damage and draw a card.
internal static class Sim_VAN_EX1_154
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
