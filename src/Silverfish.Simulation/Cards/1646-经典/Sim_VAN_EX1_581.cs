using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 闷棍
// Sap
// 将一个敌方随从移回你对手的手牌。
// Return an enemy minion to your opponent's hand.
internal static class Sim_VAN_EX1_581
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
