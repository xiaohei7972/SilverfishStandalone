using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 王牌猎人
// Big Game Hunter
// 可交易 战吼：消灭一个攻击力大于或等于7的随从。
// Tradeable Battlecry: Destroy a minion with 7 or more Attack.
internal static class Sim_EX1_005
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_MIN_ATTACK, 7), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
