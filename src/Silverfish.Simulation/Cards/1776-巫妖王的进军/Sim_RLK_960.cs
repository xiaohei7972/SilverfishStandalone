using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 力量余烬
// Embers of Strength
// 召唤两个1/2并具有嘲讽的守卫。法力渴求（6）：使其获得+1/+2。
// Summon two 1/2 Guards with Taunt. Manathirst (6): Give them +1/+2.
internal static class Sim_RLK_960
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
