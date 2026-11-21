using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 呼叫蛇颈龙群
// Call the Threshfleet!
// 使一个随从获得+2/+2和圣盾。（当伊度存活时，本牌会保留在你手中！）
// Give a minion +2/+2 and Divine Shield. (This stays in your hand while Ido is alive!)
internal static class Sim_TLC_241t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
