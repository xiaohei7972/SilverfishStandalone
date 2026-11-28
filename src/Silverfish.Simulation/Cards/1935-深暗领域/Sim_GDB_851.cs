using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 星域相变射线
// Astral Phaser
// 抉择：随机对两个敌方随从造成$2点伤害；或者使一个敌方随从休眠2回合。
// Choose One - Deal $2 damage to two random enemy minions; or Make one Dormant for 2 turns.
internal static class Sim_GDB_851
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1), new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
