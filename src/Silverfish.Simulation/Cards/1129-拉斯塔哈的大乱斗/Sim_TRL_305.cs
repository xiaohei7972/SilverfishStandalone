using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 新人登场
// A New Challenger...
// 发现一张法力值消耗为（6）的随从牌。召唤该随从并使其获得嘲讽和圣盾。
// Discover a 6-Cost minion. Summon it with Taunt and Divine Shield.
internal static class Sim_TRL_305
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
