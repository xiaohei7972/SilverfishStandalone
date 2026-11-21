using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 锐鳞骑士
// Scalerider
// 战吼：如果你的手牌中有龙牌，则造成2点伤害。
// Battlecry: If you're holding a Dragon, deal 2 damage.
internal static class Sim_DRG_081
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
