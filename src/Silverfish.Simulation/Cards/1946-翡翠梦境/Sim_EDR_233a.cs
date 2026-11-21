using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 群狼的力量
// Wolf's Strength
// 召唤三只2/3并具有嘲讽的狼。
// Summon three 2/3 Wolves with Taunt.
internal static class Sim_EDR_233a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
