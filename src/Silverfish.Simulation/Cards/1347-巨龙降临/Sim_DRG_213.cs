using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 双头暴虐龙
// Twin Tyrant
// 战吼：随机对两个敌方随从造成4点伤害。
// Battlecry: Deal 4 damage to two random enemy minions.
internal static class Sim_DRG_213
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
