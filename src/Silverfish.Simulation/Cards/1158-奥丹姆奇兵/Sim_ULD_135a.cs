using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 结识古树
// Befriend the Ancient
// 召唤一棵6/6并具有嘲讽的 古树。
// Summon a 6/6 Ancient with Taunt.
internal static class Sim_ULD_135a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
