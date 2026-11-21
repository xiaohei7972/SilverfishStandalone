using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暗影猎手沃金
// Shadow Hunter Vol'jin
// 战吼：选择一个随从，与其拥有者手牌中的随机一个随从交换。
// Battlecry: Choose a minion. Swap it with a random one in its owner's hand.
internal static class Sim_BAR_080
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
