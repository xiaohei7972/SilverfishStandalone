using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 蟾蜍雨
// Rain of Toads
// 召唤三个2/4并具有嘲讽的蟾蜍。 过载：（3）
// Summon three 2/4 Toads with Taunt. Overload: (3)
internal static class Sim_TRL_351
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
