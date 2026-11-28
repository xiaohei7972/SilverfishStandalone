using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 腐巢幼龙
// Rotnest Drake
// 战吼：如果你的手牌中有龙牌，随机消灭一个敌方随从。
// Battlecry: If you're holding a Dragon, destroy a random enemy minion.
internal static class Sim_YOD_036
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
