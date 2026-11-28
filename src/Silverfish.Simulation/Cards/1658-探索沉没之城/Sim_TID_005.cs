using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 耐普图隆的指令
// Command of Neptulon
// 召唤两个5/4并具有突袭的元素。过载：（1）
// Summon two 5/4 Elementals with Rush. Overload: (1)
internal static class Sim_TID_005
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
