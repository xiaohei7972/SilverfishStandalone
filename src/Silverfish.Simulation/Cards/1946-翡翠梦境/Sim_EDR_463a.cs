using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 操纵荆棘
// Constricting Thorns
// 消灭一个攻击力小于或等于3的随从。
// Destroy a minion with 3 or less Attack.
internal static class Sim_EDR_463a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_MAX_ATTACK, 3)],
        []);
    // </legacy-play-requirements>

}
