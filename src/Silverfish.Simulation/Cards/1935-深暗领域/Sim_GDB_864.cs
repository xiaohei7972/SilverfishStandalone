using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 第一次接触
// First Contact
// 随机召唤两个法力值消耗为（1）的随从。过载：（1）
// Summon two random 1-Cost minions. Overload: (1)
internal static class Sim_GDB_864
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
