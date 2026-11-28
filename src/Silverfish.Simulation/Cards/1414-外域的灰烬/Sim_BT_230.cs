using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 鱼斯拉
// The Lurker Below
// 战吼：对一个敌方随从造成3点伤害。如果该随从死亡，则对一个相邻的随从重复此效果。
// Battlecry: Deal 3 damage to an enemy minion. If it dies, repeat on one of its neighbors.
internal static class Sim_BT_230
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
