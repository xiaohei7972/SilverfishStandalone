using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 神圣寿司卷
// Holy Maki Roll
// 恢复#2点生命值。在本回合可以重复使用。
// Restore #2 Health. Repeatable this turn.
internal static class Sim_TSC_952
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
