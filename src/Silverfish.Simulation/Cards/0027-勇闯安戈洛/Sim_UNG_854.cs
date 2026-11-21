using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 琥口脱险
// Free From Amber
// 发现一张法力值消耗大于或等于（8）点的随从牌，并召唤该随从。
// Discover a minion that costs (8) or more. Summon it.
internal static class Sim_UNG_854
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_WITH_DEATHRATTLE)],
        []);
    // </legacy-play-requirements>

}
