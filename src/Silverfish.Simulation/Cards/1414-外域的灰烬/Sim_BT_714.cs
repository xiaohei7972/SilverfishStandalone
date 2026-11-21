using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰霜织影者
// Frozen Shadoweaver
// 战吼： 冻结一个敌人。
// Battlecry: Freeze an_enemy.
internal static class Sim_BT_714
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
