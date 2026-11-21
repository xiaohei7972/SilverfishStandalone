using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 饮用泉水
// Drink the Water
// 恢复#12点生命值。
// Restore #12 Health.
internal static class Sim_ULD_135b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
