using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暴掠龙女王
// Ravasaur Matriarch
// 延系：对一个敌方随从造成等同于本随从攻击力的伤害。
// Kindred: Deal damage to an enemy minion equal to this minion's Attack.
internal static class Sim_TLC_825
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
