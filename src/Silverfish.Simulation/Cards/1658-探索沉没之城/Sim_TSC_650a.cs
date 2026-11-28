using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 号令虎鲸
// Order the Orca
// 召唤一只6/6并具有嘲讽的虎鲸。
// Summon a 6/6 Orca with Taunt.
internal static class Sim_TSC_650a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
