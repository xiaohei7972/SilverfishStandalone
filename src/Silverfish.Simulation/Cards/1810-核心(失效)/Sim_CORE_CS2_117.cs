using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 大地之环先知
// Earthen Ring Farseer
// 战吼： 恢复#3点生命值。
// Battlecry: Restore #3_Health.
internal static class Sim_CORE_CS2_117
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
