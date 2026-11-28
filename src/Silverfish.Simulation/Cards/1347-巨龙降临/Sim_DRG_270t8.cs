using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 玛里苟斯的寒冰箭
// Malygos's Frostbolt
// 对一个角色造成$3点伤害，并使其冻结。
// Deal $3 damage to a_character and Freeze it.
internal static class Sim_DRG_270t8
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
