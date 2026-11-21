using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 咒术之箭
// Conjured Arrow
// 对一个随从造成$2点伤害。法力渴求（6）：抽取相同数量的牌。
// Deal $2 damage to a minion. Manathirst (6): Draw that many cards.
internal static class Sim_RLK_804
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
