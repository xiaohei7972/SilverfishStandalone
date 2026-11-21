using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 瓶装泉水
// Bottled Springwater
// 恢复余下的生命值。@恢复#{0}点生命值。
// Restore the excess Health.@Restore #{0} Health.
internal static class Sim_WW_395t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_DAMAGED_TARGET)],
        []);
    // </legacy-play-requirements>

}
