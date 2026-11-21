using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 净化吐息
// Purifying Breath
// 对一个随从造成$5点伤害。如果该随从死亡，则为敌方英雄恢复#5点生命值。
// Deal $5 damage to a minion. If it dies, restore #5 Health to the enemy hero.
internal static class Sim_CATA_303
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
