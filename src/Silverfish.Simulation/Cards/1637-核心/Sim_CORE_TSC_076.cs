using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 永存石中
// Immortalized in Stone
// 召唤具有嘲讽的4/8，2/4，1/2的元素各一个。
// Summon a 4/8, 2/4, and 1/2 Elemental with Taunt.
internal static class Sim_CORE_TSC_076
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
