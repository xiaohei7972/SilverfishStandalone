using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 恐惧小道
// Terror Run
// 卡库仅提供描述模板。
// Card data contains only a description template.
internal static class Sim_TLC_100t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)]);
    // </legacy-play-requirements>

}
