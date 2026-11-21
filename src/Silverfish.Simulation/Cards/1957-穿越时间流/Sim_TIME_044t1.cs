using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 现在的诺莫瑞根
// Present Gnomeregan
// 使一个随从获得+2/+1和“亡语：对敌方英雄造成2点伤害。”推进到未来！
// Give a minion +2/+1 and "Deathrattle: Deal 2 damage to the enemy hero." Advance to the future!
internal static class Sim_TIME_044t1
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)]);
    // </legacy-play-requirements>

}
