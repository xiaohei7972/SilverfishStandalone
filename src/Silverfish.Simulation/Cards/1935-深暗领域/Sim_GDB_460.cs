using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 神圣之星
// Divine Star
// 对一个随从造成$3点伤害。随机使你手牌中的一张随从牌获得+3生命值。
// Deal $3 damage to a minion. Give a random minion in your hand +3_Health.
internal static class Sim_GDB_460
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
