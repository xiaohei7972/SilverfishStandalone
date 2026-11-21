using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 风语者
// Windspeaker
// 战吼：使一个友方随从获得风怒。
// Battlecry: Give a friendly minion Windfury.
internal static class Sim_EX1_587
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
