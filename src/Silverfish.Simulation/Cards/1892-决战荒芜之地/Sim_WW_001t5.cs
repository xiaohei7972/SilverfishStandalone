using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 钟乳落石
// Falling Stalactite
// 对一个随从和敌方英雄造成$3点伤害。
// Deal $3 damage to a minion and the enemy hero.
internal static class Sim_WW_001t5
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
