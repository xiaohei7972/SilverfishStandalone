using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 宿敌
// Vendetta
// 对一个随从造成$4点伤害。如果你的手牌中有另一职业的卡牌，则法力值消耗为（0）点。
// Deal $4 damage to a minion. Costs (0) if you're holding a card from another class.
internal static class Sim_DAL_716
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
