using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 镜像
// Mirror Image
// 嘲讽
// Taunt
internal static class Sim_VAN_CS2_mirror
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
