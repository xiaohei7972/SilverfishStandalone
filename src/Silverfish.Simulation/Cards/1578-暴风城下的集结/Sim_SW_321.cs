using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 瞄准射击
// Aimed Shot
// 造成$3点伤害。你的下一个英雄技能会额外造成2点伤害。
// Deal $3 damage. Your next Hero Power deals 2 more damage.
internal static class Sim_SW_321
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
