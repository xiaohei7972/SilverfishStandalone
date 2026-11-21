using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 天降蛛群
// Ball of Spiders
// 召唤三只1/1并具有“亡语：随机获取一张野兽牌”的 结网蛛。
// Summon three 1/1 Webspinners with "Deathrattle: Get a random Beast."
internal static class Sim_AT_062
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
