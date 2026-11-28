using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 未来的诺莫瑞根
// Future Gnomeregan
// 使一个随从获得+2/+1，圣盾和“亡语：对敌方英雄造成2点伤害。”
// Give a minion +2/+1, Divine_Shield, and "Deathrattle: Deal 2 damage to the enemy hero."
internal static class Sim_TIME_044t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)]);
    // </legacy-play-requirements>

}
