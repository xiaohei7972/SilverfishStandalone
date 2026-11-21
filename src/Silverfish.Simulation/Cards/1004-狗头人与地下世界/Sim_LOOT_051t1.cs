using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 法术玉石
// Jasper Spellstone
// 对一个随从造成$4点伤害。@
// Deal $4 damage to a minion. @
internal static class Sim_LOOT_051t1
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
