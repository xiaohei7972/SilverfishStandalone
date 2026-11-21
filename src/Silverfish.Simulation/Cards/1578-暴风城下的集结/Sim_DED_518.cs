using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 操纵火炮
// Man the Cannons
// 对一个随从造成$3点伤害，并对所有其他随从造成$1点 伤害。
// Deal $3 damage to a minion and $1 damage to all other minions.
internal static class Sim_DED_518
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
