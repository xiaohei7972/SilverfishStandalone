using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 自然之怒
// Nature's Wrath
// 对一个随从造成$1点伤害，抽一张牌。
// Deal $1 damage to a minion. Draw a card.
internal static class Sim_VAN_EX1_154b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
