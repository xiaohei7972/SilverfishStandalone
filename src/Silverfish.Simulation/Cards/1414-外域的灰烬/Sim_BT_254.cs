using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 塞泰克织巢者
// Sethekk Veilweaver
// 在你对一个随从施放法术后，随机将一张牧师法术牌置入你的 手牌。
// After you cast a spell on a minion, add a Priest spell to your hand.
internal static class Sim_BT_254
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
