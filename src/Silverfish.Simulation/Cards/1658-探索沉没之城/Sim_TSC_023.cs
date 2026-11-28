using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 倒刺捕网
// Barbed Nets
// 对一个敌人造成$2点伤害。如果你在本牌在你手中时使用过纳迦牌，则再选择一个目标。
// Deal $2 damage to an enemy. If you played a Naga while holding this, choose a second target.
internal static class Sim_TSC_023
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
