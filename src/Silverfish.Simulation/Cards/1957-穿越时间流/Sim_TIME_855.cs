using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 奥术弹幕
// Arcane Barrage
// 对一个敌人造成$3点伤害，并随机对两个其他敌人造成$2点伤害。
// Deal $3 damage to an enemy and $2 damage to two other random ones.
internal static class Sim_TIME_855
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
