using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 向下深挖
// Digging Straight Down
// 对一个随从造成$8点伤害。发掘一个宝藏。
// Deal $8 damage to a minion. Excavate a treasure.
internal static class Sim_DEEP_009
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
