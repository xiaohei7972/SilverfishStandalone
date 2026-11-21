using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 赏金合约
// Lucrative Contract
// 消灭一个随从。将两张幸运币置入你的手牌。
// Destroy a minion. Add 2 Coins to your hand.
internal static class Sim_DAL_366t3
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
