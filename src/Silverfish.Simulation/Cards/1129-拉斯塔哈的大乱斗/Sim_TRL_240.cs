using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 野蛮先锋
// Savage Striker
// 战吼：对一个敌方随从造成等同于你的英雄攻击力的伤害。
// Battlecry: Deal damage to an enemy minion equal to your hero's Attack.
internal static class Sim_TRL_240
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
