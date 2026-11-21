using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 衰变飞弹
// Devolving Missiles
// 随机向敌方随从发射三枚飞弹，使其变形成为法力值消耗减少（1）点的随从。
// Shoot three missiles at random enemy minions that transform them into ones that cost (1) less.
internal static class Sim_SCH_235
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
