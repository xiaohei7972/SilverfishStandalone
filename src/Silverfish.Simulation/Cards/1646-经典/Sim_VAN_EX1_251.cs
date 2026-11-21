using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 叉状闪电
// Forked Lightning
// 随机对两个敌方随从造成$2点伤害，过载：（2）
// Deal $2 damage to 2_random enemy minions. Overload: (2)
internal static class Sim_VAN_EX1_251
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
