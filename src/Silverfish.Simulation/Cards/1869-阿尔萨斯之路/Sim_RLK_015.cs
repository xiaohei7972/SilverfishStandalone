using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 凛风冲击
// Howling Blast
// 对一个敌人造成$3点伤害并将其冻结。对所有其他敌人造成$1点伤害。
// Deal $3 damage to an enemy and Freeze it. Deal $1 damage to all other enemies.
internal static class Sim_RLK_015
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
