using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 严酷的监工
// Cruel Taskmaster
// 战吼：对一个随从造成1点伤害，并使其获得+2攻击力。
// Battlecry: Deal 1 damage to a minion and give it +2_Attack.
internal static class Sim_EX1_603
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
