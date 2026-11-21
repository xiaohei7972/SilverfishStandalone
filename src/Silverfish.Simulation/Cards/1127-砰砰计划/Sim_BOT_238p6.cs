using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 微型战队
// Micro-Squad
// 召唤三个1/1的微型机器人。每回合 切换。
// Summon three 1/1 Microbots. Swaps_each_turn.
internal static class Sim_BOT_238p6
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
