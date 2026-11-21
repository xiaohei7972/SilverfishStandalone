using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 巫毒娃娃
// Voodoo Doll
// 战吼：选择一个随从。亡语：消灭选择的随从。
// Battlecry: Choose a minion. Deathrattle: Destroy the chosen minion.
internal static class Sim_GIL_614
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
