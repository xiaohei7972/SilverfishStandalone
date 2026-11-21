using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 禁忌咒文
// Forbidden Words
// 消耗你所有的法力值。消灭一个攻击力小于或等于所消耗法力值的随从。
// Spend all your Mana. Destroy a minion with that much Attack or less.
internal static class Sim_DAL_723
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
