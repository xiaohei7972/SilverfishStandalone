using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 帆船舰长
// Sailboat Captain
// 战吼：使一个友方海盗获得风怒。
// Battlecry: Give a friendly Pirate Windfury.
internal static class Sim_VAC_937
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
