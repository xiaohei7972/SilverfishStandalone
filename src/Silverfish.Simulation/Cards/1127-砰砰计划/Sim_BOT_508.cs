using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 死金药剂
// Necrium Vial
// 触发一个友方随从的亡语两次。
// Trigger a friendly minion's Deathrattle twice.
internal static class Sim_BOT_508
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_TARGET_WITH_DEATHRATTLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
