using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 树篱迷宫
// Hedge Maze
// 触发一个友方随从的亡语。
// Trigger a friendly minion's Deathrattle.
internal static class Sim_CORE_REV_333
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_WITH_DEATHRATTLE)]);
    // </legacy-play-requirements>

}
