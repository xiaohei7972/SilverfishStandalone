using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 琥珀雏龙
// Amber Whelp
// 战吼：如果你的手牌中有龙牌，则造成3点伤害。
// Battlecry: If you're holding a Dragon, deal 3 damage.
internal static class Sim_RLK_915
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
