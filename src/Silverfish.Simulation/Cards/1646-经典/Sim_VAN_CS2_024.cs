using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 寒冰箭
// Frostbolt
// 对一个角色造成$3点伤害，并使其冻结。
// Deal $3 damage to a_character and Freeze it.
internal static class Sim_VAN_CS2_024
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
