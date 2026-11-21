using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 爆破小队
// Bomb Squad
// 战吼：对一个敌方随从造成5点伤害。 亡语：对你的英雄造成5点伤害。
// Battlecry: Deal 5 damage to an enemy minion. Deathrattle: Deal 5 damage to your hero.
internal static class Sim_CFM_667
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
