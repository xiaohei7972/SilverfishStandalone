using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 腐蚀吐息
// Corrosive Breath
// 对一个随从造成$3点伤害。如果你的手牌中有龙牌，还会命中敌方英雄。
// Deal $3 damage to a minion. If you're holding a Dragon, it also hits the enemy hero.
internal static class Sim_DRG_006
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
