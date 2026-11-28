using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 希亚玛特之心
// Siamat's Heart
// 使希亚玛特获得嘲讽。
// Give Siamat Taunt.
internal static class Sim_ULD_178a3
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
