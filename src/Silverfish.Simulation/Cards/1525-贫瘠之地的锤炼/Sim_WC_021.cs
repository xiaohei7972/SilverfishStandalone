using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 不稳定的暗影震爆
// Unstable Shadow Blast
// 对一个随从造成$6点伤害，超过目标生命值的伤害会命中你的英雄。
// Deal $6 damage to a minion. Excess damage hits your hero.
internal static class Sim_WC_021
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
