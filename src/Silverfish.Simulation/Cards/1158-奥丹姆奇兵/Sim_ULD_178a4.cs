using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 希亚玛特之速
// Siamat's Speed
// 使希亚玛特获得突袭。
// Give Siamat Rush.
internal static class Sim_ULD_178a4
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
