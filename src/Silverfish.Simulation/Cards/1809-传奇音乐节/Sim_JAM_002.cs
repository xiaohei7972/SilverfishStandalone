using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 星辰能量
// Star Power
// 随机对一个敌方随从造成$5点伤害。重复此效果，每次伤害减少1点。
// Deal $5 damage to a random enemy minion. Repeat this with 1 less damage.
internal static class Sim_JAM_002
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
