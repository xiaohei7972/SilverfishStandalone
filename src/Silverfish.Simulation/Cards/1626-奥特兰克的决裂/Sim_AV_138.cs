using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 恐怖图腾赏金猎人
// Grimtotem Bounty Hunter
// 战吼：消灭一个敌方传说随从。
// Battlecry: Destroy an enemy Legendary minion.
internal static class Sim_AV_138
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_LEGENDARY_TARGET), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
