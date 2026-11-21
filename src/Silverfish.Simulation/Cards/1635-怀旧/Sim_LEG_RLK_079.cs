using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 喷毒尸骸
// Noxious Cadaver
// 战吼： 对一个敌人和你的英雄造成2点伤害。
// Battlecry: Deal 2 damage to an enemy and your hero.
internal static class Sim_LEG_RLK_079
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
