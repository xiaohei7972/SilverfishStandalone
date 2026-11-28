using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 大地祈咒
// Earth Invocation
// 召唤两个2/3并具有嘲讽的元素。
// Summon two 2/3 Elementals with Taunt.
internal static class Sim_AV_258t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
