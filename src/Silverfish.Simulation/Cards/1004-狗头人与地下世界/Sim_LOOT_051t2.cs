using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 大型法术玉石
// Greater Jasper Spellstone
// 对一个随从造成$6点伤害。
// Deal $6 damage to a minion.
internal static class Sim_LOOT_051t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
