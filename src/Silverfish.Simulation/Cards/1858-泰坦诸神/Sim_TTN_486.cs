using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 魔化形态
// Monstrous Form
// 直到你的下个回合，使一个友方随从获得+3/+3。
// Give a friendly minion +3/+3 until your next turn.
internal static class Sim_TTN_486
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
