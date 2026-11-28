using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 断生鱿鱼
// Undercooked Calamari
// 战吼：消灭一个攻击力小于或等于本随从的敌方随从。
// Battlecry: Destroy an enemy minion with Attack less than or equal to this minion's.
internal static class Sim_VAC_341
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_MAX_ATTACK, 3)],
        []);
    // </legacy-play-requirements>

}
