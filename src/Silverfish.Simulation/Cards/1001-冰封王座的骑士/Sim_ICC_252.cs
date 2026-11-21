using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰冷鬼魂
// Coldwraith
// 战吼：如果有敌人被冻结，抽一张牌。
// Battlecry: If an enemy is Frozen, draw a card.
internal static class Sim_ICC_252
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_FROZEN_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
