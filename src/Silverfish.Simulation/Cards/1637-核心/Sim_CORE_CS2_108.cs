using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 斩杀
// Execute
// 消灭一个受伤的敌方随从。
// Destroy a damaged enemy minion.
internal static class Sim_CORE_CS2_108
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_DAMAGED_TARGET)],
        []);
    // </legacy-play-requirements>

}
