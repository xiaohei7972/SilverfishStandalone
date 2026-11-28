using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 未鉴定的合约
// Unidentified Contract
// 消灭一个随从。在你手牌中时获得额外效果。
// Destroy a minion. Gains a bonus effect in_your hand.
internal static class Sim_DAL_366
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
