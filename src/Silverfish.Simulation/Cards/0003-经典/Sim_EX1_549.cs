using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 狂野怒火
// Bestial Wrath
// 在本回合中，使一只友方野兽获得+2攻击力和免疫。
// Give a friendly Beast +2 Attack and Immune this turn.
internal static class Sim_EX1_549
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.BEAST), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
