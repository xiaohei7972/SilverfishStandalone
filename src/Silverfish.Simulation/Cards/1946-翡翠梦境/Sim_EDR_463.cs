using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暮光侵扰
// Twilight Influence
// 抉择：消灭一个攻击力小于或等于3的随从；或者随机召唤一个法力值消耗为（2）的随从。
// Choose One - Destroy a minion with 3 or less Attack; or Summon a random 2-Cost minion.
internal static class Sim_EDR_463
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_MAX_ATTACK, 3), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
