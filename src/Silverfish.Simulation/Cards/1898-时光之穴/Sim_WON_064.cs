using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暗言术：禁
// Shadow Word: Forbid
// 可交易 消灭一个攻击力为4的随从。腐蚀：消灭所有攻击力为4的随从。
// Tradeable Destroy a 4-Attack minion. Corrupt: Destroy ALL 4-Attack minions.
internal static class Sim_WON_064
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_EXACT_ATTACK, 4)],
        []);
    // </legacy-play-requirements>

}
