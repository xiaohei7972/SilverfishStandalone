using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 击剑教头
// Fencing Coach
// 战吼：你的下一个英雄技能的法力值消耗减少（2）点。
// Battlecry: The next time you use your Hero Power, it costs (2) less.
internal static class Sim_AT_115
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
