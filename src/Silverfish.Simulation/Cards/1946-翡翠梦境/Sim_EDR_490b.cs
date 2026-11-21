using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 神智的终结
// Wit's End
// 消灭一个敌方随从。
// Destroy an enemy minion.
internal static class Sim_EDR_490b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
