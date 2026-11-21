using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 恐惧坐骑
// Dreaded Mount
// 使一个随从获得+1/+1。当该随从死亡时，召唤一匹无尽的恐惧战马。
// Give a minion +1/+1. When it dies, summon an endless Dreadsteed.
internal static class Sim_SW_087
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
