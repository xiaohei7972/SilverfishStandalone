using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暗言术：灭
// Shadow Word: Death
// 消灭一个攻击力大于或等于5的随从。
// Destroy a minion with 5_or more Attack.
internal static class Sim_CORE_EX1_622
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_MIN_ATTACK, 5)],
        []);
    // </legacy-play-requirements>

}
