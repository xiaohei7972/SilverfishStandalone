using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 潜地蝎
// Burrowing Scorpid
// 战吼：造成2点伤害。如果消灭了目标，则获得潜行。
// Battlecry: Deal 2 damage. If that kills the target, gain Stealth.
internal static class Sim_BT_717
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
