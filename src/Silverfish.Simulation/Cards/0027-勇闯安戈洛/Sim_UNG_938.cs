using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 温泉守卫
// Hot Spring Guardian
// 嘲讽，战吼： 恢复#3点生命值。
// Taunt Battlecry: Restore #3_Health.
internal static class Sim_UNG_938
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
