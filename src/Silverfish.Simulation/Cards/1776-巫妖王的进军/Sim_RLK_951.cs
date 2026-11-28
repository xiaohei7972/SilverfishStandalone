using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 验尸官
// Coroner
// 战吼：冻结一个敌方随从。法力渴求（6）：先将其沉默。
// Battlecry: Freeze an enemy minion. Manathirst (6): Silence it first.
internal static class Sim_RLK_951
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
