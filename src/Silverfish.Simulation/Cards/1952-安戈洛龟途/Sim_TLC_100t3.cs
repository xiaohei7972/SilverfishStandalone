using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 火羽山
// Fire Plume Ridge
// 卡库仅提供描述模板。
// Card data contains only a description template.
internal static class Sim_TLC_100t3
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)]);
    // </legacy-play-requirements>

}
