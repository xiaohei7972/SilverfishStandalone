using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 再生
// Regenerate
// 恢复#3点生命值。
// Restore #3 Health.
internal static class Sim_TRL_128
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
