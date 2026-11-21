using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 桶滚动作
// Barrel Roll
// 对一个未受伤的角色造成$5点伤害。如果你正在构筑星舰，则法力值消耗为（1）点。
// Deal $5 damage to an undamaged character. Costs (1) if you're building a Starship.
internal static class Sim_GDB_465
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_DAMAGED_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
