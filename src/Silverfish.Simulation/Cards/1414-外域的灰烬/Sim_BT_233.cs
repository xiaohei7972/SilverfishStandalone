using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 剑盾猛攻
// Sword and Board
// 对一个随从造成$2点伤害。获得2点护甲值。
// Deal $2 damage to a minion. Gain 2 Armor.
internal static class Sim_BT_233
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
