using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 法术珍珠
// Pearl Spellstone
// 召唤一个4/4并具有嘲讽的灵魂。@
// Summon a 4/4 Spirit with Taunt. @
internal static class Sim_LOOT_091t1
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
