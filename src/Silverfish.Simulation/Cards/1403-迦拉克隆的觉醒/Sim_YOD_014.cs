using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 永世掠夺者
// Aeon Reaver
// 战吼：对一个随从造成等同于其攻击力的伤害。
// Battlecry: Deal damage to_a minion equal to its_Attack.
internal static class Sim_YOD_014
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
