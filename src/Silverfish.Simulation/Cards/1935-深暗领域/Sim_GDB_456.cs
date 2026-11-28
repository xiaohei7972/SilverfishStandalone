using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 自燃
// Spontaneous Combustion
// 随机对一个敌人造成$4点伤害。如果你在上个回合使用过元素牌，则可以选择目标。
// Deal $4 damage to a random enemy. If you played an Elemental last turn, choose the target.
internal static class Sim_GDB_456
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
