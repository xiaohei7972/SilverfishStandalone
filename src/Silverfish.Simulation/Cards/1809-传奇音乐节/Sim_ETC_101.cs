using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 牛铃独演者
// Cowbell Soloist
// 战吼：如果你没有控制其他随从，造成2点伤害。
// Battlecry: If you control no other minions, deal 2 damage.
internal static class Sim_ETC_101
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
