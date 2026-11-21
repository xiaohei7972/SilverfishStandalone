using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 炭火
// Char
// 对一个随从造成$7点伤害。随机使你手牌中的一张随从牌获得等同于超过目标生命值伤害的属性值。
// Deal $7 damage to a minion. Give a random minion in your hand stats equal to the excess damage.
internal static class Sim_VAC_526
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
