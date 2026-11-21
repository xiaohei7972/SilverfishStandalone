using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 最后期限
// Deadline
// 可交易，临时 消灭一个随从。
// Tradeable, Temporary Destroy a minion.
internal static class Sim_WORK_007
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TRADEABLE)],
        []);
    // </legacy-play-requirements>

}
