using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 凋零缠绕
// Death Coil
// 对一个敌人造成$5点伤害，或为一个友方角色恢复#5点 生命值。
// Deal $5 damage to an enemy, or restore #5 Health to a friendly character.
internal static class Sim_ICC_314t5
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
