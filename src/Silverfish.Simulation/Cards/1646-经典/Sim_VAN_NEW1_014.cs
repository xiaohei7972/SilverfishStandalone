using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 伪装大师
// Master of Disguise
// 战吼：使一个友方随从获得潜行。
// Battlecry: Give a friendly minion Stealth.
internal static class Sim_VAN_NEW1_014
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
