using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 真言术：宴
// Power Word: Feast
// 使一个随从获得+2/+2。在本回合结束时，使其恢复所有生命值。
// Give a minion +2/+2. Restore it to full Health at the end of this turn.
internal static class Sim_SCH_136
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
