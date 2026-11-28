using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 地狱火！
// INFERNO!
// 英雄技能 召唤一个6/6的 地狱火。
// Hero Power Summon a 6/6 Infernal.
internal static class Sim_VAN_EX1_tk33
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
