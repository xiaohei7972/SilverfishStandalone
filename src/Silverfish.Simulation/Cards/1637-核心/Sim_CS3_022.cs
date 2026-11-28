using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 雾帆劫掠者
// Fogsail Freebooter
// 战吼：如果你装备着武器，造成2点伤害。
// Battlecry: If you have a weapon equipped, deal_2_damage.
internal static class Sim_CS3_022
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
