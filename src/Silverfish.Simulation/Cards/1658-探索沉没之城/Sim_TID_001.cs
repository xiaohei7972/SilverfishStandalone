using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 月光射线
// Moonbeam
// 对一个敌人造成$1点伤害两次。
// Deal $1 damage to an enemy, twice.
internal static class Sim_TID_001
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
