using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 恐惧畏缩
// Cower in Fear
// 对一个随从造成$3点伤害。在本回合中，你使用的下一张野兽牌法力值消耗减少（2）点。
// Deal $3 damage to a minion. The next Beast you play this turn costs (2) less.
internal static class Sim_TLC_823
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
