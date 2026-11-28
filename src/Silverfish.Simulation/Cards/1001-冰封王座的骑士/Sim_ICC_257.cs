using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 唤尸者
// Corpse Raiser
// 战吼：使一个友方随从获得“亡语：再次召唤该随从。”
// Battlecry: Give a friendly minion "Deathrattle: Resummon this minion."
internal static class Sim_ICC_257
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
