using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 雷德·黑手
// Rend Blackhand
// 战吼：如果你的手牌中有龙牌，则消灭一个传说随从。
// Battlecry: If you're holding a Dragon, destroy a Legendary minion.
internal static class Sim_BRM_029
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_LEGENDARY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
