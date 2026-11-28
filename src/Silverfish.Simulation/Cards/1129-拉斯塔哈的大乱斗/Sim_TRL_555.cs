using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 恶魔之箭
// Demonbolt
// 消灭一个随从。你每控制一个随从，本牌的法力值消耗便减少（1）点。
// Destroy a minion. Costs (1) less for each minion you control.
internal static class Sim_TRL_555
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
