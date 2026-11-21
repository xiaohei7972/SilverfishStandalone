using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 任务助理
// Questing Assistant
// 战吼：如果你在本局对战中使用过任务牌，对一个敌方随从造成3点伤害。
// Battlecry: If you played a Quest this game, deal 3 damage to an enemy minion.
internal static class Sim_TLC_987
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
