using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 起爆炸药
// Blast Charge
// 消灭一个受伤的敌方随从。发掘一个宝藏。
// Destroy a damaged enemy minion. Excavate a treasure.
internal static class Sim_WW_380
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_DAMAGED_TARGET)],
        []);
    // </legacy-play-requirements>

}
