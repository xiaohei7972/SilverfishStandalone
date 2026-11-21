using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 夜幕奇袭
// Gang Up
// 选择一个随从。将该随从的三张复制洗入你的牌库。
// Choose a minion. Shuffle 3 copies of it into your deck.
internal static class Sim_BRM_007
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
