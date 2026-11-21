using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 淹没
// Drown
// 将一个敌方随从置于你的牌库底。
// Put an enemy minion on the bottom of your deck.
internal static class Sim_TID_920
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
