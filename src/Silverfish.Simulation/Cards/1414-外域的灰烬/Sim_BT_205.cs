using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 废铁射击
// Scrap Shot
// 造成$3点伤害。随机使你手牌中的一张野兽牌获得+3/+3。
// Deal $3 damage. Give a random Beast in_your hand +3/+3.
internal static class Sim_BT_205
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
