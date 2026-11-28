using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 古老的秘密
// Ancient Secrets
// 恢复5点生命值。
// Restore 5 Health.
internal static class Sim_VAN_NEW1_008b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
