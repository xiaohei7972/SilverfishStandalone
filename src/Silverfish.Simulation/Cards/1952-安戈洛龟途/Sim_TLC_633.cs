using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 害虫克星
// Bugsquasher
// 战吼：对一个具有随从类型的敌方随从造成6点伤害。
// Battlecry: Deal 6 damage to an enemy minion with a minion type.
internal static class Sim_TLC_633
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_MUST_HAVE_TAG, GAME_TAG.CARDRACE), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
