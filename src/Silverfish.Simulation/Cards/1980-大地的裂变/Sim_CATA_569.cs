using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 演武仪式
// Ceremonial Clash
// 随机召唤法力值消耗为（3），（2）和（1）的随从各一个。过载：（1）
// Summon a random 3, 2, and 1-Cost minion. Overload: (1)
internal static class Sim_CATA_569
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
