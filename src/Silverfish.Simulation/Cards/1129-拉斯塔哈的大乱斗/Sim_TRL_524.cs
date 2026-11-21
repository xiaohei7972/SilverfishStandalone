using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 破盾者
// Shieldbreaker
// 战吼：沉默一个具有嘲讽的敌方随从。
// Battlecry: Silence an enemy minion with Taunt.
internal static class Sim_TRL_524
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MUST_TARGET_TAUNTER), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
