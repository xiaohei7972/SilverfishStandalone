using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 战槌挑战者
// Warmaul Challenger
// 战吼： 选择一个敌方随从。与其战斗至死！
// Battlecry: Choose an enemy minion. Battle it to the death!
internal static class Sim_BT_120
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
