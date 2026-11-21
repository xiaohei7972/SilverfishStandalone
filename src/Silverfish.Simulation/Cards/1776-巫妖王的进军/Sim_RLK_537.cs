using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 扭曲束缚
// Twisted Tether
// 消灭一个随从，随机使你手牌中的一张亡灵牌获得其属性值。
// Destroy a minion. Give its stats to a random Undead in your hand.
internal static class Sim_RLK_537
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
