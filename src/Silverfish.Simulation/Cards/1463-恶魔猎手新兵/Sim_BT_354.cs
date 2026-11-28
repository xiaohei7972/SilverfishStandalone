using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 刃舞
// Blade Dance
// 随机对三个敌方随从造成等同于你的英雄攻击力的伤害。
// Deal damage equal to your hero's Attack to 3 random enemy minions.
internal static class Sim_BT_354
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE_AND_HERO_HAS_ATTACK), new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
