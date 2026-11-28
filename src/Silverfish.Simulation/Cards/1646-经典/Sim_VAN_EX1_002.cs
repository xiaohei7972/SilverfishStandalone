using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 黑骑士
// The Black Knight
// 战吼：消灭一个具有嘲讽的敌方随从。
// Battlecry: Destroy an enemy minion with Taunt.
internal static class Sim_VAN_EX1_002
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_MUST_TARGET_TAUNTER), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
