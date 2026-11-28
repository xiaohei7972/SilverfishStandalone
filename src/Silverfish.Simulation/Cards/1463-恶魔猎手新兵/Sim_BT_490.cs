using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 吞噬魔法
// Consume Magic
// 沉默一个敌方随从。流放：抽一张牌。
// Silence an enemy_minion. Outcast: Draw a card.
internal static class Sim_BT_490
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
