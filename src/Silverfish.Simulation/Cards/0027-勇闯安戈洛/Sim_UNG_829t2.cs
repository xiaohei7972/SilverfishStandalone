using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 虚空传送门
// Nether Portal
// 在你的回合结束时，召唤两个3/2的小鬼。
// At the end of your turn, summon two 3/2 Imps.
internal static class Sim_UNG_829t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
