using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 两栖之灵
// Amphibian's Spirit
// 使一个随从获得+2/+2和“亡语：使一个友方随从获得+2/+2以及此亡语。”
// Give a minion +2/+2 and "Deathrattle: Give a friendly minion +2/+2 and this Deathrattle."
internal static class Sim_EDR_261
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
