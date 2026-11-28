using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 横扫
// Swipe
// 对一个敌人造成$4点伤害，并对所有其他敌人 造成$1点伤害。
// Deal $4 damage to an enemy and $1 damage to all other enemies.
internal static class Sim_CORE_CS2_012
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
