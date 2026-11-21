using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 眼镜蛇射击
// Cobra Shot
// 对一个随从和敌方英雄造成$3点伤害。
// Deal $3 damage to a minion and the enemy hero.
internal static class Sim_WON_306
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
