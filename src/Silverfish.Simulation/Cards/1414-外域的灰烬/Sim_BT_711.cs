using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 钉棍终结者
// Blackjack Stunner
// 战吼：如果你控制一个奥秘，将一个随从移回其拥有者的手牌，并且法力值消耗增加（2）点。
// Battlecry: If you control a Secret, return a minion to its owner's hand. It costs (2) more.
internal static class Sim_BT_711
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_SECRETS, 1), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
