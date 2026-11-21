using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 愈合之花
// Healing Blossom
// 恢复#5点生命值。
// Restore #5 Health.
internal static class Sim_DAL_350b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
