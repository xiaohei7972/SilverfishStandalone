using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 致命射线
// Lethal Rays
// 随机对两个敌方随从造成$2点 伤害。
// Deal $2 damage to two random enemy minions.
internal static class Sim_GDB_851a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
