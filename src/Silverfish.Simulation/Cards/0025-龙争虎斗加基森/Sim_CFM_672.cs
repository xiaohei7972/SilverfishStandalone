using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 郭雅夫人
// Madam Goya
// 战吼：选择一个友方随从，召唤你牌库中的它的所有复制。
// Battlecry: Choose a friendly minion. Summon any copies of it in your deck.
internal static class Sim_CFM_672
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
