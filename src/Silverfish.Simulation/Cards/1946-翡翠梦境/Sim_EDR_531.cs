using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 虹吸生长
// Siphoning Growth
// 消灭一个友方随从以获得8点护甲值。
// Destroy a friendly minion to gain 8 Armor.
internal static class Sim_EDR_531
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
