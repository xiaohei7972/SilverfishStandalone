using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 寒霜冰爆元素
// Frostshatter Elemental
// 战吼：冻结一个敌人。抽两张牌。
// Battlecry: Freeze an enemy. Draw two cards.
internal static class Sim_JAIL_803t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_NONSTEALTH_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
