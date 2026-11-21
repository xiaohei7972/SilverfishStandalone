using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 神圣坐骑
// Noble Mount
// 使一个随从获得+1/+1和圣盾。当该随从死亡时，召唤一匹战马。
// Give a minion +1/+1 and Divine Shield. When it dies, summon a Warhorse.
internal static class Sim_SW_316
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
