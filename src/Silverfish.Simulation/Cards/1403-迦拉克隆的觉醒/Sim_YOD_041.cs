using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 风暴之眼
// Eye of the Storm
// 召唤三个5/6并具有嘲讽的元素。 过载：（3）
// Summon three 5/6 Elementals with Taunt. Overload: (3)
internal static class Sim_YOD_041
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
