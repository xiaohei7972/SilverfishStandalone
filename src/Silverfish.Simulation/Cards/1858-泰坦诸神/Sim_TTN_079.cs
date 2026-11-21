using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 虫外有虫
// Always a Bigger Jormungar
// 使一个随从获得+2攻击力和“超过目标生命值的攻击伤害会命中敌方英雄。”
// Give a minion +2_Attack and "Excess damage dealt by attacks hits the enemy hero."
internal static class Sim_TTN_079
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
