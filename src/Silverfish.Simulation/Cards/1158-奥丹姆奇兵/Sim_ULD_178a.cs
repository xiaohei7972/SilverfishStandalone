using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 希亚玛特之风
// Siamat's Wind
// 使希亚玛特获得风怒。
// Give Siamat Windfury.
internal static class Sim_ULD_178a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
