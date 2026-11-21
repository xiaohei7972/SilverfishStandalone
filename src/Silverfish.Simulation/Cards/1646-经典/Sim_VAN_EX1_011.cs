using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 巫医
// Voodoo Doctor
// 战吼： 恢复#2点生命值。
// Battlecry: Restore #2_Health.
internal static class Sim_VAN_EX1_011
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
