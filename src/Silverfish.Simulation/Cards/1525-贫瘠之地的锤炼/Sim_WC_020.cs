using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 永恒之火
// Perpetual Flame
// 随机对一个敌方随从造成$3点伤害。如果该随从死亡，再次施放本法术。过载：（1）
// Deal $3 damage to a random enemy minion. If it dies, recast this. Overload: (1)
internal static class Sim_WC_020
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
