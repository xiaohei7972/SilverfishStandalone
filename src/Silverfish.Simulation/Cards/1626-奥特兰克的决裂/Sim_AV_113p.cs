using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 召唤宠物
// Summon Pet
// 召唤一个动物伙伴。
// Summon an Animal Companion.
internal static class Sim_AV_113p
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
