using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 群体狂乱
// Mass Hysteria
// 使每个随从随机攻击其他随从。
// Force each minion to_attack another random minion.
internal static class Sim_TRL_258
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
