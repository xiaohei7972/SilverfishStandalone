using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 潜入
// Infiltrate
// 选择一个随从。对所有其他随从造成$3点伤害。
// Choose a minion. Deal $3 damage to all other minions.
internal static class Sim_GDB_902
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
