using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 集体诉讼律师
// Class Action Lawyer
// 战吼：如果你的牌库中没有中立卡牌，将一个随从的属性值变为1/1。
// Battlecry: If your deck has no Neutral cards, set _a minion's stats to 1/1.
internal static class Sim_MAW_017
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
