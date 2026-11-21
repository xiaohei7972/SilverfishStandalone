using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 女巫杂酿
// Witch's Brew
// 恢复#4点生命值。在本回合可以重复使用。
// Restore #4 Health. Repeatable this turn.
internal static class Sim_DAL_432
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
