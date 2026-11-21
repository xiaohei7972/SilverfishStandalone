using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 雷象坐骑
// Elekk Mount
// 使一个随从获得+4/+7和嘲讽。当该随从死亡时，召唤一只雷象。
// Give a minion +4/+7 and Taunt. When it dies, summon an Elekk.
internal static class Sim_SW_443
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
