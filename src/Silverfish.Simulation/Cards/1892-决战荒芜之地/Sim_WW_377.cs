using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 热浪来袭
// Heat Wave
// 对一个敌方随从及其相邻随从造成$2点伤害。快枪：改为对所有敌人。
// Deal $2 damage to an enemy minion and its neighbors. Quickdraw: To all enemies instead.
internal static class Sim_WW_377
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
