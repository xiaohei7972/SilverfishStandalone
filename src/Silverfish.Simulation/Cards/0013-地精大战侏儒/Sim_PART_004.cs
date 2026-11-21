using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 隐秘力场
// Finicky Cloakfield
// 直到你的下个回合，使一个友方随从获得潜行。
// Give a friendly minion Stealth until your next turn.
internal static class Sim_PART_004
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
