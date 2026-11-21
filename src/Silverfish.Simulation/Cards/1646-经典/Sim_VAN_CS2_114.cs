using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 顺劈斩
// Cleave
// 随机对两个敌方随从造成 $2点伤害。
// Deal $2 damage to two random enemy minions.
internal static class Sim_VAN_CS2_114
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
