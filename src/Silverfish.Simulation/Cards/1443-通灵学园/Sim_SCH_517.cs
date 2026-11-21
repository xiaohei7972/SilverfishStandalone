using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 影光学者
// Shadowlight Scholar
// 战吼：摧毁一张你牌库中的灵魂残片，造成3点伤害。
// Battlecry: Destroy a Soul Fragment in your deck to deal 3 damage.
internal static class Sim_SCH_517
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
