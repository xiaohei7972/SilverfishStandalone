using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 创世之力
// Power of Creation
// 发现一张法力值消耗为（6）的随从牌。召唤两个它的 复制。
// Discover a 6-Cost minion. Summon two copies of it.
internal static class Sim_DAL_578
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
