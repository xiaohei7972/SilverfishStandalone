using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 沟渠潜伏者
// Moat Lurker
// 战吼：消灭一个随从。亡语：再次召唤被消灭的随从。
// Battlecry: Destroy a minion. Deathrattle: Resummon it.
internal static class Sim_KAR_041
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
