using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 采矿事故
// Mining Casualties
// 召唤两个{0}并具有“亡语：召唤一个1/1的脆弱的食尸鬼”的白银之手新兵。
// Summon two {0} Silver Hand Recruits with "Deathrattle: Summon a 1/1 Frail Ghoul".
internal static class Sim_DEEP_017
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
