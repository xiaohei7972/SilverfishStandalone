using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 审判恶徒
// Judge Unworthy
// 将一个敌方随从的生命值变为1，然后对所有敌人造成$1点 伤害。
// Set an enemy minion's Health to 1, then deal $1 damage to all enemies.
internal static class Sim_TTN_853
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
