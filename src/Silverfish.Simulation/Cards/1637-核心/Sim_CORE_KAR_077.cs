using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 银月城传送门
// Silvermoon Portal
// 使一个随从获得+2/+2。随机召唤一个法力值消耗为（2）的随从。
// Give a minion +2/+2. Summon a random 2-Cost minion.
internal static class Sim_CORE_KAR_077
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
