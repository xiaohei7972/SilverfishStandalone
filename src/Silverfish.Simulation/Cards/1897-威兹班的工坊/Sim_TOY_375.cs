using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 滑冰元素
// Sleet Skater
// 微缩 战吼：冻结一个敌方随从，获得等同于其攻击力的护甲值。
// Miniaturize Battlecry: Freeze an enemy minion. Gain Armor equal to its Attack.
internal static class Sim_TOY_375
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
