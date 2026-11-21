using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 生命撕裂者
// Liferender
// 战吼：在本回合中，如果你的英雄的生命值发生变化，对一个敌方随从造成6点伤害。
// Battlecry: If your hero's Health changed this turn, deal 6 damage to an enemy minion.
internal static class Sim_TIME_614
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE_AND_PLAYER_HEALTH_CHANGED_THIS_TURN), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
