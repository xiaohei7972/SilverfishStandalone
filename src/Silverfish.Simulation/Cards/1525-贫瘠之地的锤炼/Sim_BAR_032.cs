using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 穿刺射击
// Piercing Shot
// 对一个随从造成$6点伤害，超过目标生命值的伤害会命中敌方英雄。
// Deal $6 damage to a minion. Excess damage hits the enemy hero.
internal static class Sim_BAR_032
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
