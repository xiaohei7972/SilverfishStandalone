using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 黑翼腐蚀者
// Blackwing Corruptor
// 战吼：如果你的手牌中有龙牌，则造成5点伤害。
// Battlecry: If you're holding a Dragon, deal 5 damage.
internal static class Sim_BRM_034
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND)],
        []);
    // </legacy-play-requirements>

}
