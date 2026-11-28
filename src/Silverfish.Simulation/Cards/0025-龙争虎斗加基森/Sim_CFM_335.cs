using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 驮运科多兽
// Dispatch Kodo
// 战吼：造成等同于本随从攻击力的伤害。
// Battlecry: Deal damage equal to this minion's Attack.
internal static class Sim_CFM_335
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
