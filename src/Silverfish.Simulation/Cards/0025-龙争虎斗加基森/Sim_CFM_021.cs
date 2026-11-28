using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰冻药水
// Freezing Potion
// 冻结一个敌人。
// Freeze an enemy.
internal static class Sim_CFM_021
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
