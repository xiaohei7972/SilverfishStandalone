using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 精神灼烧
// Mind Sear
// 对一个随从造成$2点伤害。如果该随从死亡，则对敌方英雄造成$3点伤害。
// Deal $2 damage to a minion. If it dies, deal $3 damage to the enemy hero.
internal static class Sim_NX2_019
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
