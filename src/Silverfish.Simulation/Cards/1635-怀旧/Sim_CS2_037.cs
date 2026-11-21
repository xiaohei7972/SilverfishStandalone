using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰霜震击
// Frost Shock
// 对一个敌方角色造成$1点伤害，并使其冻结。
// Deal $1 damage to an enemy character and Freeze it.
internal static class Sim_CS2_037
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
