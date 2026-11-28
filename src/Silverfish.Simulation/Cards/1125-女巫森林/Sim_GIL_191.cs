using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 恶魔法阵
// Fiendish Circle
// 召唤四个1/1的小鬼。
// Summon four 1/1 Imps.
internal static class Sim_GIL_191
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
