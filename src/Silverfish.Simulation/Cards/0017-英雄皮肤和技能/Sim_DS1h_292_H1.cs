using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 稳固射击
// Steady Shot
// 英雄技能 对敌方英雄造成$2点伤害。@英雄技能 造成$2点伤害。
// Hero Power Deal $2 damage to the enemy hero.@Hero Power Deal $2 damage.
internal static class Sim_DS1h_292_H1
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_OR_ENEMY_HERO), new(ErrorType.REQ_STEADY_SHOT)],
        []);
    // </legacy-play-requirements>

}
