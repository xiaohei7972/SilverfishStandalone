using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 奥尔多卫士
// Aldor Peacekeeper
// 战吼：使一个敌方随从的攻击力变为1。
// Battlecry: Change an_enemy minion's Attack to 1.
internal static class Sim_VAN_EX1_382
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
