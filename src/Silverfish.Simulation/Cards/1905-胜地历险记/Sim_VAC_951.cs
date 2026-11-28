using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// “健康”饮品
// "Health" Drink
// 吸血 对一个随从造成$3点伤害。（还剩3杯！）
// Lifesteal. Deal $3 damage to a minion. (3 Drinks left!)
internal static class Sim_VAC_951
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
