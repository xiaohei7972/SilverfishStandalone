using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 亡语者
// Deathspeaker
// 战吼：在本回合中，使一个友方随从获得免疫。
// Battlecry: Give a friendly minion Immune this turn.
internal static class Sim_ICC_467
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
