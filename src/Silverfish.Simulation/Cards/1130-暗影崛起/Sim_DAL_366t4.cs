using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 叛变合约
// Turncoat Contract
// 消灭一个随从，并使其对相邻的随从造成等同于其攻击力的伤害。
// Destroy a minion. It_deals its damage to adjacent minions.
internal static class Sim_DAL_366t4
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
