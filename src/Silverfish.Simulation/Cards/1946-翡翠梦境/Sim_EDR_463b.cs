using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 控制藤蔓
// Controlling Vines
// 随机召唤一个法力值消耗为（2）的随从。
// Summon a random 2-Cost minion.
internal static class Sim_EDR_463b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
