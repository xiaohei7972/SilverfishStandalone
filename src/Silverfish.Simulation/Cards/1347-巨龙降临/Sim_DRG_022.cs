using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 横冲直撞
// Ramming Speed
// 迫使一个随从攻击相邻的一个 随从。
// Force a minion to attack one of its neighbors.
internal static class Sim_DRG_022
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
