using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 大型法术珍珠
// Greater Pearl Spellstone
// 召唤一个6/6并具有嘲讽的 灵魂。
// Summon a 6/6 Spirit with Taunt.
internal static class Sim_LOOT_091t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
