using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 黑暗私语
// Dark Wispers
// 抉择： 召唤5个小精灵；或者使一个随从获得+5/+5和嘲讽。
// Choose One - Summon 5 Wisps; or Give_a minion +5/+5 and Taunt.
internal static class Sim_GVG_041
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
