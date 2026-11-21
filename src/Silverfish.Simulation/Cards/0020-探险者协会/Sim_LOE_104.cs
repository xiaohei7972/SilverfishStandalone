using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 埋葬
// Entomb
// 选择一个敌方随从。将该随从洗入你的牌库。
// Choose an enemy minion. Shuffle it into your deck.
internal static class Sim_LOE_104
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
