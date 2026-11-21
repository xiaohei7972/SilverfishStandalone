using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 守卫执勤
// Guard Duty
// 随机召唤法力值消耗为（6），（4）和（2）的嘲讽随从各一个。
// Summon a random 6, 4, and 2-Cost Taunt minion.
internal static class Sim_DINO_433
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
