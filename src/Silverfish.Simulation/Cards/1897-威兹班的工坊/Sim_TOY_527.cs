using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 诅咒之旅
// Cursed Campaign
// 使一个友方随从获得“亡语：召唤本随从的两个休眠2回合的复制。”
// Give a friendly minion "Deathrattle: Summon two copies of this minion that are Dormant for 2 turns."
internal static class Sim_TOY_527
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
