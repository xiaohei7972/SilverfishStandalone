using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 胁迫
// Coerce
// 消灭一个受伤的随从。连击：消灭任意随从。
// Destroy a damaged minion. Combo: Destroy any minion.
internal static class Sim_SCH_521
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_DAMAGED_TARGET), new(ErrorType.REQ_DAMAGED_TARGET_UNLESS_COMBO)],
        []);
    // </legacy-play-requirements>

}
