using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 多重射击
// Multi-Shot
// 随机对两个敌方随从造成$3点 伤害。
// Deal $3 damage to two random enemy minions.
internal static class Sim_VAN_DS1_183
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
