using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰川急冻
// Glaciate
// 发现一张法力值消耗为（8）的随从牌。召唤并冻结该随从。
// Discover an 8-Cost minion. Summon and Freeze it.
internal static class Sim_AV_107
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_WITH_DEATHRATTLE)],
        []);
    // </legacy-play-requirements>

}
