using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暗言术：窃
// Shadow Word: Steal
// 将一个敌方随从移回你的手牌。
// Return an enemy minion to YOUR hand.
internal static class Sim_DEEP_021
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
