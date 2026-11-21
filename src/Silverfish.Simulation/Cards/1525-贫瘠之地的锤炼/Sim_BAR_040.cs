using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 南海岸酋长
// South Coast Chieftain
// 战吼：如果你控制着其他鱼人，则造成2点伤害。
// Battlecry: If you control another Murloc, deal 2_damage.
internal static class Sim_BAR_040
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
