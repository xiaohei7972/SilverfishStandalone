using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 蛇颈龙骑手的祝福
// Threshrider's Blessing
// 使一个友方随从获得+4/+4和“亡语：随机召唤一个法力值消耗为（4）的随从。”
// Give a friendly minion +4/+4 and "Deathrattle: Summon a random 4-Cost minion."
internal static class Sim_TLC_477
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
