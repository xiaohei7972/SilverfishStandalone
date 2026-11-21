using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 急速冷冻
// Snap Freeze
// 冻结一个随从。如果该随从已被冻结，则将其消灭。
// Freeze a minion. If it's already Frozen, destroy it.
internal static class Sim_CORE_GIL_801
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
