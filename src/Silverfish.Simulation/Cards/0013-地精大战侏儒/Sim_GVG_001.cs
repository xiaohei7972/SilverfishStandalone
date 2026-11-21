using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 烈焰轰击
// Flamecannon
// 随机对一个敌方随从造成 $4点伤害。
// Deal $4 damage to a random enemy minion.
internal static class Sim_GVG_001
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
