using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 猢狲医者
// Hozen Healer
// 战吼：为一个随从恢复所有生命值。
// Battlecry: Restore a minion to full Health.
internal static class Sim_CFM_067
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
