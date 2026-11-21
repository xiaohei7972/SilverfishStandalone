using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 神圣赞美诗
// Divine Hymn
// 为所有友方角色恢复#6点 生命值。
// Restore #6 Health to all friendly characters.
internal static class Sim_GIL_661
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
