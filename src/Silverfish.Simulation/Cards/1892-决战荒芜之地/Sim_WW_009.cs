using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 低温贮藏
// Cryopreservation
// 冻结一个敌人。发掘一个 宝藏。
// Freeze an enemy. Excavate a treasure.
internal static class Sim_WW_009
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
