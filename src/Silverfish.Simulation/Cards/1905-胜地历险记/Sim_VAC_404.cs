using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 夜影花茶
// Nightshade Tea
// 对一个随从造成$2点伤害。对你的英雄造成$2点伤害。（还剩3杯！）
// Deal $2 damage to a minion. Deal $2 damage to your hero. (3 Drinks left!)
internal static class Sim_VAC_404
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
