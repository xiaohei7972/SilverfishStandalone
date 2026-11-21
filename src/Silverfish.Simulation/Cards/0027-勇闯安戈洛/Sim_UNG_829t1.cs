using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 虚空传送门
// Nether Portal
// 打开一座会永久召唤3/2小鬼的传送门。
// Open a permanent portal that summons 3/2 Imps.
internal static class Sim_UNG_829t1
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
