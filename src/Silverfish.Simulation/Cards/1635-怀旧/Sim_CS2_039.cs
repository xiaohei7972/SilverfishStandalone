using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 风怒
// Windfury
// 使一个随从获得风怒。
// Give a minion Windfury.
internal static class Sim_CS2_039
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
