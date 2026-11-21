using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 梦境
// Dream
// 将一个随从移回其拥有者的 手牌。
// Return a minion to its owner's hand.
internal static class Sim_VAN_DREAM_04
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
