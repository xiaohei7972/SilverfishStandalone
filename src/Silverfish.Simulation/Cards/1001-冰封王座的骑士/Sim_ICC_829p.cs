using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 天启四骑士
// The Four Horsemen
// 召唤一个2/2的天启骑士。如果你控制所有四个天启骑士，消灭敌方英雄。
// Summon a 2/2 Horseman. If you have all 4, destroy the enemy hero.
internal static class Sim_ICC_829p
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
