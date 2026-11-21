using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 军情七处的要挟
// SI:7 Extortion
// 可交易 对一个未受伤的角色造成$3点伤害。
// Tradeable Deal $3 damage to an undamaged character.
internal static class Sim_SW_412
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_UNDAMAGED_TARGET)],
        []);
    // </legacy-play-requirements>

}
