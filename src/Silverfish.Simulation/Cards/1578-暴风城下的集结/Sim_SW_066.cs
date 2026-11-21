using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 王室图书管理员
// Royal Librarian
// 可交易 战吼：沉默一个 随从。
// Tradeable Battlecry: Silence a minion.
internal static class Sim_SW_066
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
