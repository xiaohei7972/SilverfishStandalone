using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 蘑菇酿酒师
// Shroom Brewer
// 战吼： 恢复#4点生命值。
// Battlecry: Restore #4_Health.
internal static class Sim_LOOT_291
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
