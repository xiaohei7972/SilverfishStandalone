using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 夜色镇炼金师
// Darkshire Alchemist
// 战吼： 恢复#5点生命值。
// Battlecry: Restore #5 Health.
internal static class Sim_OG_234
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
