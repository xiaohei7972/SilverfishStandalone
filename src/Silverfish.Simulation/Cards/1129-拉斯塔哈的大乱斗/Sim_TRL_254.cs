using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 神灵印记
// Mark of the Loa
// 抉择： 使一个随从获得+2/+4和嘲讽；或者召唤两个3/2的迅猛龙。
// Choose One - Give a minion +2/+4 and Taunt; or Summon two 3/2 Raptors.
internal static class Sim_TRL_254
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
