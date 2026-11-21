using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 废土守望者
// Waste Warden
// 战吼： 对一个随从及所有相同类型的其他随从造成3点伤害。
// Battlecry: Deal 3 damage to a minion and all others of the same minion type.
internal static class Sim_BT_729
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
