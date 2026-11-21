using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 金丝雀
// Canary
// 战吼：将一个 敌方随从移回其拥有者的手牌。
// Battlecry: Return an enemy minion to its owner's hand.
internal static class Sim_WW_001t7
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
