using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 疯狂药水
// Potion of Madness
// 直到回合结束，获得一个攻击力小于或等于2的敌方随从的控制权。
// Gain control of an enemy minion with 2 or less Attack until end of turn.
internal static class Sim_CFM_603
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_NUM_MINION_SLOTS, 1), new(ErrorType.REQ_TARGET_MAX_ATTACK, 2)],
        []);
    // </legacy-play-requirements>

}
