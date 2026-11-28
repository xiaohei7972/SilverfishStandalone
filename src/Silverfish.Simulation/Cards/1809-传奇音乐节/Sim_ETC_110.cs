using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 封面艺人
// Cover Artist
// 战吼：变形成为一个随从的3/3的复制。
// Battlecry: Transform into a 3/3 copy of a minion.
internal static class Sim_ETC_110
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
