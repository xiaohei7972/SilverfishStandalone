using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 希亚玛特之盾
// Siamat's Shield
// 使希亚玛特获得圣盾。
// Give Siamat Divine Shield.
internal static class Sim_ULD_178a2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
