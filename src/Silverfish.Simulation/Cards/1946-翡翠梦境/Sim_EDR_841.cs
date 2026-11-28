using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 恐魂腐蚀者
// Dreadsoul Corrupter
// 战吼，亡语：&随机召唤一个休眠的魔种。
// Battlecry and Deathrattle:& Summon a random Dormant Dreadseed.
internal static class Sim_EDR_841
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 2)],
        []);
    // </legacy-play-requirements>

}
